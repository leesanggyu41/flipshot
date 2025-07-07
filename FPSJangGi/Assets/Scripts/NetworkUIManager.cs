 using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class NetworkUIManager : MonoBehaviourPunCallbacks
{
    public GameObject serverConnectPanel;
    public GameObject lobbyPanel;
    public TMP_Text statusText;
    public TMP_InputField roomCodeInput;

    public AudioSource aa;

    public GameObject naem;

    private void Start()
    {
        serverConnectPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        statusText.text = "서버 접속 중...";
        PhotonNetwork.ConnectUsingSettings();

        if (PhotonNetwork.NetworkClientState == ClientState.JoinedLobby)
        {
            serverConnectPanel.SetActive(false);
            aa.Play();
            lobbyPanel.SetActive(true);
        }
    }

    public override void OnConnectedToMaster()      
    {
        Debug.Log("마스터 서버 접속 성공!");
        statusText.text = "서버 연결 완료, 로비 입장 중...";

        if (!PhotonNetwork.InLobby) // 현재 로비에 있지 않다면
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장 완료!");
        statusText.text = "로비 입장 완료!";
        aa.Play();
        serverConnectPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    private bool IsNicknameSet()
    {
        if (string.IsNullOrEmpty(PhotonNetwork.NickName))
        {
            naem.SetActive(true);
            Invoke("aaaa", 1f);
            Debug.LogError("닉네임을 설정해야 합니다!");
            return false;
        }
        return true;
    }

    void aaaa()
    {
        naem.SetActive(false);
    }

    public void CreateRoom()
    {
        if (!IsNicknameSet()) return;
        string roomCode = Random.Range(1000, 9999).ToString();
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 2, IsVisible = true, IsOpen = true };
        PhotonNetwork.CreateRoom(roomCode, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 완료! 방 번호: " + PhotonNetwork.CurrentRoom.Name);
       
    }

    public void JoinRoom()
    {
        if (!IsNicknameSet()) return;
        string roomCode = roomCodeInput.text;
        if (!string.IsNullOrEmpty(roomCode))
        {
            PhotonNetwork.JoinRoom(roomCode);
        }
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
