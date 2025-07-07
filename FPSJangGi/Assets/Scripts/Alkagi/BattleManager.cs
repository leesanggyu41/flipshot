using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

public class BattleManager : MonoBehaviourPun
{
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
    public Rigidbody my; //내가 조종한 돌
    public Rigidbody your; //맞은 상대돌
    public bool sex = false;
    public bool isgo = false;   
    public GameObject cutScene;
    public PhotonView turnManagerView;

    public AlkagiStone han;
    public AlkagiStone cho;

    public Transform hancam;
    public Transform chocam;
    private ResultManger resultManger;
    private TurnManager turnManager;
    private BattleSpawner battleSpawner;

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
    public void Start()
    {
        count = FindAnyObjectByType<TMP_Text>();
        resultManger = FindAnyObjectByType<ResultManger>();
        turnManager = FindAnyObjectByType<TurnManager>();
        battleSpawner = FindAnyObjectByType<BattleSpawner>();

        turnManagerView = turnManager.GetComponent<PhotonView>();

    }

    [PunRPC]
    private void Update()
    {
        if (hancam == null || chocam == null)
        {
            GameObject Hancam = GameObject.FindWithTag("hancam");
            hancam = Hancam.transform;
            GameObject Chocam = GameObject.FindWithTag("chocam");
            chocam = Chocam.transform;
        }


        if (hancount >= 16)
        {
            StartCoroutine(chowin());
        }
        if(chocount >= 16)
        {
            StartCoroutine(hanwin());
        }
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
        //배틀 시작 애니메이션 시작
        cutScene.SetActive(true);

        yield return new WaitForSeconds(15f);
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
        Debug.Log("알까기무브활성화");
        hancam.position = new Vector3(0, 7f, 0);
        chocam.position = new Vector3(0, 7f, 0);
        Pan.des();
        StartCoroutine(strenght());

    }


    public IEnumerator strenght()
    {
        endbattle = true;
        yield return new WaitForSeconds(1f);
        ball.Stop();
        alka.Play();
        Debug.Log("스트랭스함수활성화");
        if (resultManger.Win == false)
        {
            Invoke("WRWR", 0.5f);
            if (turnManager.isMasterTurn == false )
            {
                Debug.Log("내 포스 :" + MyForce + "적 포스" + YouForce);

                //my.AddForce(MyForceDirection.normalized * MyForce, ForceMode.Impulse);
                my.AddForce(MyForceDirection.normalized * MyForce, ForceMode.Impulse);
                usshot = false;
                isbattle = false;
                
            }
            else if(turnManager.isMasterTurn != false )
            {
                Debug.Log("내 포스 :" + MyForce + "적 포스" + YouForce);
                my.AddForce(MyForceDirection.normalized * MyForce, ForceMode.Impulse);
                usshot = false;
                isbattle = false;
                
            }
        }
        else
        {
            Invoke("WRWR", 0.5f);
            Debug.Log("내 포스 :" + MyForce + "적 포스" + YouForce);
            if (turnManager.isMasterTurn == false )
            {
                //my.AddForce(MyForceDirection.normalized * MyForce, ForceMode.Impulse);
                your.AddForce(YouForceDirection.normalized * YouForce, ForceMode.Impulse);
                usshot = false;
                isbattle = false;
                
            }
            else if (turnManager.isMasterTurn != false )
            {
                Debug.Log("내 포스 :" + MyForce + "적 포스" + YouForce);
                your.AddForce(YouForceDirection.normalized * YouForce, ForceMode.Impulse);
                usshot = false;
                isbattle = false;
                
            }
        }


    }


    void WRWR()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (turnManager.isMasterTurn)
            {

                turnManagerView.RPC("EndTurnRPC", RpcTarget.All);
            }
        }
        else
        {
            if (!turnManager.isMasterTurn)
            {
                turnManagerView.RPC("EndTurnRPC", RpcTarget.All);
            }
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