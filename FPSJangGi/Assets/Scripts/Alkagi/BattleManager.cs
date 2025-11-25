using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject turntool;
    public GameObject effect;
    private TMP_Text count;
    public bool isbattle = false;
    public bool endbattle = false;
    public bool canmove = false;
    public Vector3 Force;
    public Vector3 YouForceDirection;
    public Vector3 MyForceDirection;
    public float YouForce;
    public float MyForce;
    public Rigidbody my; // 내가 조종한 돌
    public Rigidbody your; // 맞은 상대돌
    public bool sex = false;
    public bool isgo = false;
    public GameObject cutScene;

    public AlkagiStone han;
    public AlkagiStone cho;

    public Transform hancam;
    public Transform chocam;
    private ResultManger resultManger;
    private TurnManager turnManager;
    private BattleSpawner battleSpawner;

    private bool masterAgreedToSkip = false;
    private bool playerAgreedToSkip = false;
    public const float CutSceneDuration = 15f;

    private bool masterAgreedToSD = false; // 마스터 클라이언트의 서든 데스 동의 여부
    private bool playerAgreedToSD = false; // 일반 플레이어의 서든 데스 동의 여부
    public bool isSuddenDeath = false;
    public GameObject SuddenDeath;

    public GameObject Hanwin;
    public GameObject Chowin;

    public int hancount = 0;
    public int chocount = 0;

    public pan Pan;

    public GameObject endscene;
    public Animator ending;

    public TMP_Text winname;

    public AudioSource alka;
    public AudioSource ball;
    public bool usshot = false;
    public bool ws = false;
    public bool isup = false;

    [SerializeField] private GameObject turn;
    [SerializeField] private GameObject Quit;
    public int myViewID = -1;
    public int yourViewID = -1;

    void Start()
    {
        count = FindAnyObjectByType<TMP_Text>();
        resultManger = FindAnyObjectByType<ResultManger>();
        turnManager = FindAnyObjectByType<TurnManager>();
        battleSpawner = FindAnyObjectByType<BattleSpawner>();
    }

    private void Update()
    {
        if (hancam == null || chocam == null)
        {
            GameObject Hancam = GameObject.FindWithTag("hancam");
            hancam = Hancam.transform;
            GameObject Chocam = GameObject.FindWithTag("chocam");
            chocam = Chocam.transform;
        }
        if (!isSuddenDeath && Input.GetKeyDown(KeyCode.P))
        {
            bool isMaster = PhotonNetwork.IsMasterClient;

            // 이미 동의했는지 확인 (불필요한 RPC 호출 방지)
            if ((isMaster && masterAgreedToSD) || (!isMaster && playerAgreedToSD))
            {
                return; // 이미 동의했다면 리턴
            }

            // RPC를 호출하여 모든 클라이언트에게 서든 데스 동의 상태를 알림
            GetComponent<PhotonView>().RPC("AgreeToSuddenDeathRPC", RpcTarget.All, isMaster);
        }

        if (hancount >= 16)
        {
            turntool.SetActive(false);
            StartCoroutine(chowin());
        }
        if (chocount >= 16)
        {
            turntool.SetActive(false);
            StartCoroutine(hanwin());
        }

        if (sex && Input.GetKeyDown(KeyCode.S))
        {
            // 현재 로컬 플레이어가 마스터 클라이언트인지 확인
            bool isMaster = PhotonNetwork.IsMasterClient;

            // 이미 동의했는지 확인 (불필요한 RPC 호출 방지)
            if ((isMaster && masterAgreedToSkip) || (!isMaster && playerAgreedToSkip))
            {
                return; // 이미 동의했다면 리턴
            }

            // RPC를 호출하여 모든 클라이언트에게 스킵 동의 상태를 알림
            GetComponent<PhotonView>().RPC("AgreeToSkipRPC", RpcTarget.All, isMaster);
        }
    }

    [PunRPC]
    public void SyncBattleForces(
    Vector3 forceFromShot,
    float youForce, Vector3 youDir,
    float myForce, Vector3 myDir,
    int myID, int yourID)
    {
        // 이 RPC는 RpcTarget.MasterClient로 호출되었으므로 MasterClient에서 실행됩니다.
        if (!PhotonNetwork.IsMasterClient) return;

        // 1. 힘 정보 저장
        Force = forceFromShot; // OnMouseUp에서 발사된 힘
        YouForce = youForce;
        YouForceDirection = youDir;
        MyForce = myForce;
        MyForceDirection = myDir;

        // 2. ViewID 저장
        myViewID = myID;
        yourViewID = yourID;

        Debug.Log($"[Master] 간소화된 힘 동기화 완료. Your Force: {YouForce}, My Force: {MyForce}");


    }

    [PunRPC]
    public void AgreeToSkipRPC(bool isMaster)
    {
        // 마스터 클라이언트가 보낸 RPC라면
        if (isMaster)
        {
            masterAgreedToSkip = true;
            Debug.Log("마스터 클라이언트가 스킵에 동의했습니다.");
        }
        // 일반 클라이언트가 보낸 RPC라면
        else
        {
            playerAgreedToSkip = true;
            Debug.Log("일반 클라이언트가 스킵에 동의했습니다.");
        }

        // 두 플레이어 모두 동의했다면 컷씬 스킵 코루틴 호출
        if (masterAgreedToSkip && playerAgreedToSkip)
        {
            // 컷씬 코루틴이 실행 중일 때만 스킵
            if (sex)
            {
                // StopCoroutine(setgo()); // 현재 실행 중인 setgo 코루틴을 중지
                // 대신, setgo 코루틴 내에서 스킵 상태를 확인하고 탈출하도록 수정할 예정입니다.
                Debug.Log("모두 동의! 컷씬을 스킵합니다.");
            }
        }
    }
    [PunRPC]
    public void AgreeToSuddenDeathRPC(bool isMaster)
    {
        // 마스터 클라이언트가 보낸 RPC라면
        if (isMaster)
        {
            masterAgreedToSD = true;
            Debug.Log("마스터 클라이언트가 서든 데스에 동의했습니다.");
        }
        // 일반 클라이언트가 보낸 RPC라면
        else
        {
            playerAgreedToSD = true;
            Debug.Log("일반 클라이언트가 서든 데스에 동의했습니다.");
        }

        // 두 플레이어 모두 동의했다면 서든 데스 시작
        if (masterAgreedToSD && playerAgreedToSD)
        {
            Debug.Log("모두 동의! 서든 데스를 시작합니다.");

            // 서든 데스 시작 RPC 호출 (모든 클라이언트에서 실행)
            GetComponent<PhotonView>().RPC("StartSuddenDeathRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void StartSuddenDeathRPC()
    {
        // 이미 서든 데스 모드라면 중복 실행 방지
        if (isSuddenDeath) return;

        isSuddenDeath = true;

        // 1. 카운트 15로 설정
        hancount = 15;
        chocount = 15;

        // 2. UI 등으로 서든 데스 시작 알림 (선택 사항: 텍스트 UI가 있다면 활용)
        SuddenDeath.SetActive(true);
        Invoke("offdeath", 3);
        // StartCoroutine(ClearTextAfterDelay(3f));

        Debug.Log("서든 데스 모드 활성화! 한: " + hancount + ", 초: " + chocount);

        // 동의 상태 초기화 (다음 기회에 다시 동의할 필요 없음)
        masterAgreedToSD = false;
        playerAgreedToSD = false;
    }

    public void offdeath()
    {
        SuddenDeath.SetActive(false);
    }

    [PunRPC]
    public void StartBattle()
    {
        StartCoroutine(setgo());

        hancam.position = new Vector3(45, 7, 0);
        chocam.position = new Vector3(45, 7, 0);
    }

    IEnumerator setgo()
    {
        sex = true;
        isbattle = true;
        alka.Stop();
        cutScene.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < CutSceneDuration)
        {
            // 두 플레이어 모두 동의했다면 루프 탈출
            if (masterAgreedToSkip && playerAgreedToSkip)
            {
                Debug.Log("컷씬 스킵됨.");
                break;
            }

            elapsedTime += 1f;
            yield return new WaitForSeconds(1f);
        }
        cutScene.SetActive(false);
        isgo = true;
        ball.Play();
        int set = 3;
        while (set > 0)
        {
            count.text = set.ToString();
            yield return new WaitForSeconds(1f);
            set--;
        }

        Cursor.SetCursor(Resources.Load<Texture2D>("Cross"), new Vector2(32, 32), CursorMode.Auto);
        count.text = "시작!";
        yield return new WaitForSeconds(1f);
        count.text = "";
        sex = false;
        canmove = true;
    }

    [PunRPC]
    public void Alkagimove()
    {
        Cursor.SetCursor(Resources.Load<Texture2D>("NC"), new Vector2(0, 0), CursorMode.Auto);
        isgo = false;
        hancam.position = new Vector3(0, 7f, 0);
        chocam.position = new Vector3(0, 7f, 0);
        Pan.des();
        StartCoroutine(strenght());
    }

    [PunRPC]
    public void Alkagimove1()
    {
        Cursor.SetCursor(Resources.Load<Texture2D>("NC"), new Vector2(0, 0), CursorMode.Auto);
        isgo = false;
        hancam.position = new Vector3(0, 7f, 0);
        chocam.position = new Vector3(0, 7f, 0);
        Pan.des();
        StartCoroutine(strenght());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Quit.SetActive(true);
        StartCoroutine(tomain());
    }

    public IEnumerator tomain()
    {
        yield return new WaitForSeconds(5);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    IEnumerator strenght()
    {
        endbattle = true;
        yield return new WaitForSeconds(1f);

        ball.Stop();
        alka.Play();
        Debug.Log("스트랭스함수활성화");

        // 0.5초 후 턴 종료
        Invoke("WRWR", 0.5f);

        // BattleManager의 PhotonView로 RPC 호출
        PhotonView pv = GetComponent<PhotonView>();
        if (pv != null)
        {
            if (turnManager.isMasterTurn && PhotonNetwork.IsMasterClient)
            {
                ApplyForceRPC(resultManger.Win, myViewID, yourViewID);                
            }
            else if(!turnManager.isMasterTurn && !PhotonNetwork.IsMasterClient)
            {
                ApplyForceRPC(resultManger.Win, myViewID, yourViewID);
            }

        }
    }

    //IEnumerator strenght1()
    //{
    //    endbattle = true;
    //    yield return new WaitForSeconds(1f);

    //    ball.Stop();
    //    alka.Play();
    //    Debug.Log("스트랭스함수활성화");

    //    Invoke("WRWR", 0.5f);
    //    usshot = false;
    //    isbattle = false;
    //}

    // 승리 여부에 따라 힘 적용
    [PunRPC]
    public void ApplyForceRPC(bool iWon, int myID, int yourID)
    {
        Debug.Log("dhkfk");


        // MasterClient 시점에서는 '나'는 항상 MasterClient, '상대'는 일반 클라이언트입니다.

        // =========================================================
        // 1. 내가 졌을 때 (iWon == false): 내 돌(마스터)에 반동 적용
        // =========================================================
        if (!iWon)
        {
            // 이 RPC는 MasterClient만 실행하므로, MasterClient의 돌에 힘을 가해야 합니다.
            PhotonView myPV = PhotonView.Find(myID);
            Debug.Log("내 돌");
            if (myPV != null)
            {
                Debug.Log("내돌2");
                Rigidbody myRB = myPV.GetComponent<Rigidbody>();
                if (myRB != null)
                {
                    // MasterClient는 소유권에 관계없이 힘을 가할 수 있지만, 
                    // myID는 충돌을 일으킨 돌의 ID이며, MasterClient가 쏜 돌이므로 Master 소유일 가능성이 높음
                    myRB.AddForce(MyForceDirection.normalized * MyForce, ForceMode.Impulse);
                    Debug.Log("[Master] 내 돌(Master)에 반동 적용됨.");
                }
            }
        }
        // =========================================================
        // 2. 내가 이겼을 때 (iWon == true): 상대 돌(일반 클라이언트)에 힘 적용
        // =========================================================
        else
        {
            // 이 RPC는 MasterClient만 실행하므로, 일반 클라이언트의 돌에 힘을 가해야 합니다.
            PhotonView yourPV = PhotonView.Find(yourID);

            if (yourPV != null)
            {

                Rigidbody yourRB = yourPV.GetComponent<Rigidbody>();
                if (yourRB != null)
                {
                    // MasterClient는 소유권에 관계없이 힘을 가할 수 있습니다.
                    yourRB.AddForce(YouForceDirection.normalized * YouForce, ForceMode.Impulse);
                    Debug.Log("[Master] 상대 돌(Non-Master)에 힘 적용됨.");
                }
            }
        }
        photonView.RPC("resettt", RpcTarget.AllBuffered);

    }

    [PunRPC]
    public void resettt()
    {
        // 상태 초기화
        usshot = false;
        isbattle = false;
        myViewID = -1;
        yourViewID = -1;
        my = null;
        your = null;
        MyForce = 0f;
        YouForce = 0f;
        MyForceDirection = Vector3.zero;
        YouForceDirection = Vector3.zero;
        masterAgreedToSkip = false;
        playerAgreedToSkip = false;
    }


    void WRWR()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (turnManager.isMasterTurn)
                turnManager.GetComponent<PhotonView>().RPC("EndTurnRPC", RpcTarget.All);
        }
        else
        {
            if (!turnManager.isMasterTurn)
                turnManager.GetComponent<PhotonView>().RPC("EndTurnRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    public void endBattle()
    {
        endbattle = false;
        battleSpawner.sexton = false;
    }

    [PunRPC]
    public void handie()
    {
        hancount++;
    }

    [PunRPC]
    public void chodie()
    {
        chocount++;
    }

    IEnumerator chowin()
    {
        endscene.SetActive(true);
        ending.SetInteger("canwin", 2);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.IsMasterClient)
            {
                winname.text = player.NickName + "승리!";
                break;
            }
        }

        yield return new WaitForSeconds(15);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    IEnumerator hanwin()
    {
        endscene.SetActive(true);
        ending.SetInteger("canwin", 1);
        winname.text = PhotonNetwork.MasterClient.NickName + "승리!";
        yield return new WaitForSeconds(15);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
