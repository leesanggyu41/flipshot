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
        statusText.text = "���� ���� ��...";
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
        Debug.Log("������ ���� ���� ����!");
        statusText.text = "���� ���� �Ϸ�, �κ� ���� ��...";

        if (!PhotonNetwork.InLobby) // ���� �κ� ���� �ʴٸ�
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ���� �Ϸ�!");
        statusText.text = "�κ� ���� �Ϸ�!";
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
            Debug.LogError("�г����� �����ؾ� �մϴ�!");
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
        Debug.Log("�� ���� �Ϸ�! �� ��ȣ: " + PhotonNetwork.CurrentRoom.Name);
       
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
        Debug.Log("�� ���� ����! �� ��ȣ: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("�� ���� ����: " + message);
    }
}
