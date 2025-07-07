using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 방 목록을 관리하고 UI를 업데이트하는 매니저
/// </summary>
public class RoomListManager : MonoBehaviourPunCallbacks
{
    public GameObject roomItemPrefab; // 방 UI 프리팹
    public Transform content; // ScrollView의 Content 영역
    public Button refreshButton; // 새로고침 버튼

    public List<RoomInfo> roomList = new List<RoomInfo>(); // 로비에서 받은 방 정보 목록

    public GameObject neam;
    private void Start()
    {
        // 새로고침 버튼의 클릭 이벤트 연결
        refreshButton.onClick.AddListener(RefreshRoomList);
    }

    /// <summary>
    /// 방 목록 새로고침 버튼 클릭 시 호출
    /// </summary>
    public void RefreshRoomList()
    {
        if (PhotonNetwork.InLobby)
        {
            Debug.Log("방 목록 새로고침 요청!");
            PhotonNetwork.LeaveLobby(); // 현재 로비를 나가고
            StartCoroutine(RejoinLobbyAndUpdate()); // 다시 로비에 접속
        }
    }
    
    private bool IsNicknameSet()
    {
        if (string.IsNullOrEmpty(PhotonNetwork.NickName))
        {
             neam.SetActive(true);
            Invoke("aaaa", 1f);
            Debug.LogError("닉네임을 설정해야 합니다!");
            return false;
        }
        return true;
    }
    void aaaa()
    {
        neam.SetActive(false);
    }
    /// <summary>
    /// 로비 재접속 후 방 목록 업데이트
    /// </summary>
    IEnumerator RejoinLobbyAndUpdate()
    {
        yield return new WaitUntil(() => !PhotonNetwork.InLobby); // 로비 나가기 완료 대기
        PhotonNetwork.JoinLobby(); // 다시 로비 접속
    }

    /// <summary>
    /// 방 목록이 업데이트되었을 때 호출됨
    /// </summary>
    public override void OnRoomListUpdate(List<RoomInfo> updatedRoomList)
    {
        roomList = updatedRoomList.Where(room => room.PlayerCount > 0).ToList();
        Debug.Log("방 목록 업데이트됨. 현재 방 수: " + roomList.Count);
        UpdateRoomListUI();
    }

    /// <summary>
    /// UI에 현재 방 목록을 반영
    /// </summary>
    private void UpdateRoomListUI()
    {
        // 기존 UI 항목 삭제
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 방 정보를 UI에 추가
        foreach (RoomInfo room in roomList)
        {
            GameObject roomItemObj = Instantiate(roomItemPrefab, content); // 프리팹 생성
            RoomItem roomItem = roomItemObj.GetComponent<RoomItem>(); // RoomItem 컴포넌트 가져오기
            roomItem.Setup(room.Name, room.PlayerCount, room.MaxPlayers, this); // 방 정보 설정
        }
    }

    /// <summary>
    /// 사용자가 방 참가를 요청할 때 호출
    /// </summary>
    public void JoinRoom(string roomName)
    {
        if (!IsNicknameSet()) return;
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 참가 성공! 방 번호: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("방 참가 실패: " + message);
    }
}
