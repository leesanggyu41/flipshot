using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))] // PhotonView가 필수 컴포넌트임을 명시
public class RoomUIManager : MonoBehaviourPunCallbacks
{
    public GameObject roomPanel; // 방 UI 전체 패널

    [Header("Player UI")]
    public TMP_Text hostNameText; // 방장 이름 표시
    public TMP_Text guestNameText; // 참가자 이름 표시
    public TMP_Text ListText; // 방 번호 등 표시

    [Header("Buttons")]
    public Button startGameButton; // 방장용 시작 버튼
    public Button readyButton; // 참가자용 준비 버튼

    [Header("Countdown")]
    public TMP_Text countdownText; // 게임 시작 카운트다운 표시
    public RoomListManager roomListManager; // 방 목록 매니저 참조

    private bool isGuestReady = false; // 게스트가 준비됐는지 여부
    private PhotonView view; // PhotonView 참조

    private bool stran;
    void Start()
    {
        // PhotonView 컴포넌트 가져오기
        

        view = GetComponent<PhotonView>();
        if (view == null)
        {
            Debug.LogError("PhotonView가 연결되지 않았습니다!");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 참가 성공! 방 번호: " + PhotonNetwork.CurrentRoom.Name);

        // 마스터 클라이언트 정보 확인
        if(PhotonNetwork.MasterClient != null)
        {
            Debug.Log("마스터 클라이언트: " + PhotonNetwork.MasterClient.NickName);
        }
        else
        {
            Debug.LogWarning("마스터 클라이언트를 찾을 수 없습니다.");
        }
        // 방 UI 패널 보이기
        roomPanel.SetActive(true);

        // 방 번호 표시
        ListText.text = $"방 번호: {PhotonNetwork.CurrentRoom.Name}";

        // 플레이어 정보 UI 표시
        UpdatePlayerUI();
        SetupUI();
    }
    public override void OnLeftRoom()
    {   
        if(!PhotonNetwork.IsMasterClient)
        {
            guestNameText.text = "";
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.JoinLobby();
        }
        else if(PhotonNetwork.IsMasterClient)
        {
            hostNameText.text = "";
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.JoinLobby();
            SetupUI();
        }
        roomPanel.SetActive(false);
        Debug.Log("방에서 나갔습니다.");
        
    }
    
    // 초기 UI 설정qpdf
    void SetupUI()
    {
        // 방장이면 시작 버튼 활성화, 참가자는 준비 버튼 활성화
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        readyButton.gameObject.SetActive(!PhotonNetwork.IsMasterClient);

        // 방장은 처음엔 시작 불가 (게스트 준비 필요)
        startGameButton.interactable = false;

        // 카운트다운 텍스트 초기화    
        countdownText.text = "";

        // 이름 설정
        hostNameText.text = PhotonNetwork.IsMasterClient ? PhotonNetwork.NickName : "";
        guestNameText.text = PhotonNetwork.IsMasterClient ? "" : PhotonNetwork.NickName;

        // 버튼에 이벤트 연결
        if (PhotonNetwork.IsMasterClient)
        {
            // 방장은 시작 버튼 클릭 시 카운트다운 시작
            startGameButton.onClick.AddListener(() =>
            {
                if (!stran)
                {
                    stran = true;
                Debug.Log(" [HOST] 시작 버튼 클릭됨 -> StartCountdown RPC 호출 시도");
                view.RPC("StartCountdown", RpcTarget.All);
                }

            });
        }
        else
        {
            // 참가자는 준비 버튼 클릭 시 방장에게 준비 완료 알림
            readyButton.onClick.AddListener(() =>
                view.RPC("PlayerReady", RpcTarget.MasterClient));
        }
    }
    public void asd()
    {
        Debug.Log("버튼을 누름!");
    }
    [PunRPC]
    void SyncPlayerNames(string hostName, string guestName)
    {
        Debug.Log($"[RPC] 닉네임 동기화: 호스트({hostName}), 게스트({guestName})");

        // UI에 닉네임 반영
        hostNameText.text = hostName;
        guestNameText.text = guestName;
    }

    // 플레이어 목록 UI 업데이트
    void UpdatePlayerUI()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트가 닉네임 정보를 수집
            string hostName = PhotonNetwork.MasterClient.NickName;
            string guestName = "";

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (!player.IsMasterClient)
                {
                    guestName = player.NickName;
                    break;
                }
            }

            // 모든 클라이언트에게 닉네임 동기화
            view.RPC("SyncPlayerNames", RpcTarget.All, hostName, guestName);
        }
    }

    // 새 플레이어가 방에 들어왔을 때 UI 갱신
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerUI();                                                                                                                                                                                                                                                                                   
    }

    // 플레이어가 나갔을 때 UI 갱신 및 상태 초기화
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerUI();

        if (PhotonNetwork.IsMasterClient)
        {
            // 게스트가 나가면 준비 상태 초기화, 시작 비활성화
            isGuestReady = false;
            startGameButton.interactable = false;
        }
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"[MasterClient Switched] 새로운 마스터: {newMasterClient.NickName}");

        // 내가 새 마스터인 경우 UI 재설정
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("내가 새 마스터입니다. SetupUI 다시 실행");
            SetupUI();             // 시작/준비 버튼 UI 재설정
            UpdatePlayerUI();      // 닉네임도 다시 동기화
        }
    }

    // 참가자가 준비 완료 눌렀을 때 실행됨 (RPC로 호출됨)
    [PunRPC]
    void PlayerReady()
    {
        Debug.Log("게스트가 준비 완료를 눌렀습니다. 이 로그는 방장 클라이언트에서 보여야 합니다.");

        isGuestReady = true;

        if (startGameButton != null)
        {
            startGameButton.interactable = true;
            Debug.Log("startGameButton 활성화됨.");
        }
        else
        {
            Debug.LogError("startGameButton이 null입니다!");
        }
    }

    // 카운트다운 시작 (모든 클라이언트에게 동기화됨)
    [PunRPC]
    void StartCountdown()
    {
        Debug.Log("[RPC] StartCountdown 호출됨! -> 코루틴 시작");
        StartCoroutine(CountdownCoroutine());
    }

    // 카운트다운 진행 코루틴
    IEnumerator CountdownCoroutine()
    {
        int count = 3;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        countdownText.text = "시작";
        yield return new WaitForSeconds(0.5f);

        // 게임 씬으로 이동 (모든 클라이언트가 동일하게)
        PhotonNetwork.LoadLevel("BattleScene");
    }
}
