using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject roomPanel; // 방 UI 패널
    public Text roomNumberText; // 방 번호 표시
    public Text playerListText; // 플레이어 목록 표시
    public Button startButton; // 시작 버튼

    void Start()
    {
        // 방 UI 초기화
        roomPanel.SetActive(false);

        if (PhotonNetwork.InRoom)
        {
            ShowRoomUI();
        }
    }

    public void ShowRoomUI()
    {
        // 방 UI 활성화
        roomPanel.SetActive(true);

        // 방 번호 표시
        roomNumberText.text = "방 번호: " + PhotonNetwork.CurrentRoom.Name;

        // 마스터이면 시작 버튼 활성화, 아니면 비활성화
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        // 플레이어 목록 업데이트
        UpdatePlayerList();
    }

    // 방에 있는 플레이어 목록을 업데이트
    void UpdatePlayerList()
    {
        playerListText.text = "플레이어 목록:\n";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }
    }

    // 플레이어가 방에 들어올 때 UI 업데이트
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdatePlayerList();
    }

    // 플레이어가 방에서 나갈 때 UI 업데이트
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UpdatePlayerList();
    }

    // 마스터가 시작 버튼을 누르면 전투 씬으로 이동
    public void StartBattle()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("마스터가 전투를 시작합니다.");
            PhotonNetwork.LoadLevel("BattleScene");
        }
        else
        {
            Debug.Log("마스터가 아닙니다. 전투를 시작할 수 없습니다.");
        }
    }
}