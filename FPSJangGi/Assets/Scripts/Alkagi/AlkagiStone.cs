using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(PhotonTransformView))]
public class AlkagiStone : MonoBehaviourPun
{
    private string owner;  // 돌의 소유자 (ActorNumber string)
    private Rigidbody rb;
    public bool isShot = false;
    private Vector3 dragStart;
    private Vector3 dragEnd;
    public float forceMultiplier = 10f;
    public float maxDragDistance = 60f;
    private float shotCheckDelay = 0.5f; // 검사 지연 시간(초)
    private float shotTime = 0f;         // 돌을 쏜 시간
    private LineRenderer lineRenderer;
    public TurnManager turnManager;  // 턴 매니저
    public BattleManager battleManager;
    public BattleSpawner battleSpawner;
    private Plane dragPlane;
    private static Material sharedLineMaterial;
    public PhotonView turnManagerView;



    private bool isDragging = false;
    private int youID;

    private Vector3 dragVector;
    private Vector3 force;

    [PunRPC]
    public void SetOwner(string playerActorNumber)
    {
        owner = playerActorNumber;
        Debug.Log($"[SetOwner] owner가 ActorNumber {playerActorNumber}로 설정됨");
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        battleManager = FindAnyObjectByType<BattleManager>();
        battleSpawner = FindAnyObjectByType<BattleSpawner>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        if (sharedLineMaterial == null)
            sharedLineMaterial = new Material(Shader.Find("Sprites/Default"));


        turnManager = FindAnyObjectByType<TurnManager>();

        dragPlane = new Plane(Vector3.up, Vector3.zero);  // 수평면 기준 드래그

        // 소유자 지정: 마스터 클라이언트면 마스터에게, 아니면 로컬 플레이어에게
        Player initialOwner = PhotonNetwork.IsMasterClient ? PhotonNetwork.MasterClient : PhotonNetwork.LocalPlayer;
        photonView.RPC("SetOwner", RpcTarget.AllBuffered, initialOwner.ActorNumber.ToString());
    }

    void OnMouseDown()
    {
        if (turnManager.turing) return;
        if (battleManager.isup) return;
        if (battleManager.isbattle) return;
        if (battleManager.usshot) return;
        Debug.Log("클릭함");
        Debug.Log(owner);
        if (!CanControl()) return;
        if (turnManager.isMasterTurn && transform.parent.CompareTag("cho")) return;
        else if (!turnManager.isMasterTurn && transform.parent.CompareTag("han")) return;
        Debug.Log("cancontrol 작동함");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            dragStart = ray.GetPoint(enter);
        }
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!CanControl() || !isDragging) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (isDragging && Input.GetMouseButtonDown(1))
        {
            CancelDrag();
        }
        if (dragPlane.Raycast(ray, out float enter))
        {
            dragEnd = ray.GetPoint(enter);
            if (turnManager.isMasterTurn)
                dragVector = dragStart - dragEnd;
            else
                dragVector = dragEnd - dragStart;
            float dragDistance = Mathf.Min(dragVector.magnitude, maxDragDistance);
            Vector3 forceDir = dragVector.normalized;
            Vector3 predictedTarget = transform.position + (forceDir * dragDistance * 1f);
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, predictedTarget);
        }
    }

    void OnMouseUp()
    {
        if (!CanControl() || !isDragging) return;
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            dragEnd = ray.GetPoint(enter);
            if (turnManager.isMasterTurn)
                dragVector = dragStart - dragEnd;
            else
                dragVector = dragEnd - dragStart;
            float dragDistance = Mathf.Min(dragVector.magnitude, maxDragDistance);
            force = dragVector.normalized * dragDistance * forceMultiplier;
            rb.AddForce(force, ForceMode.Impulse);
            float y;

            y = Random.Range(0f, 1f);

            Vector3 dir = new Vector3(0f, y, 0f);
            rb.AddTorque(dir * 0.2f, ForceMode.Impulse);
            isShot = true;
            shotTime = Time.time; // 쏜 시간 기록
            lineRenderer.positionCount = 0;
            battleManager.isup = true; // 코루틴 시작
            StartCoroutine(turnover());
        }
        battleManager.usshot = true;

    }
    private void CancelDrag()
    {
        isDragging = false;
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
        battleManager.isup = false; 


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground")) return;
        if (collision.transform.CompareTag("dest")) return ;

        if (collision.transform.parent == null) return;

        if (battleManager.isbattle) return;
        if (!battleManager.isup) return;

        if (PhotonNetwork.IsMasterClient)
        {
            if (turnManager.isMasterTurn && !isShot) return;
           
        }
        else
        {
            if(!turnManager.isMasterTurn && !isShot) return;
        }




        if (battleManager.endbattle) return;


        
        string myTeamTag = transform.parent.tag;
        string otherTeamTag = collision.transform.parent.tag;

        if (myTeamTag == otherTeamTag) return;
        string hantag;
        string chotag;
        if (turnManager.isMasterTurn)
        {
            hantag = gameObject.tag;
            chotag = collision.transform.tag;
        }
        else
        {
            chotag = gameObject.tag;
            hantag = collision.transform.tag;
        }



        // 충돌한 Rigidbody
        Rigidbody otherRb = collision.rigidbody;
        if (otherRb == null) return;
        battleManager.isbattle = true;
        PhotonView battleManagerView = battleManager.GetComponent<PhotonView>();
        PhotonView battleSpawnerView = battleSpawner.GetComponent<PhotonView>();
        Debug.Log("EKr!");



        if (gameObject.transform.parent.CompareTag("han"))
        {
            Vector3 relativeVelocity = collision.relativeVelocity;
            Vector3 impulse = collision.impulse;
            Vector3 direction = impulse.normalized;  // 내가 준 힘 방향
            Vector3 reverseDirection = -direction;   // 내가 받은 반작용 방향
            battleManager.Force = force;
            battleManager.YouForce = impulse.magnitude ;
            battleManager.YouForceDirection = direction;
            battleManager.MyForce = impulse.magnitude ;
            battleManager.MyForceDirection = reverseDirection;
            Rigidbody aRb = gameObject.GetComponent<Rigidbody>();
            Rigidbody bRb = collision.gameObject.GetComponent<Rigidbody>();
            battleManager.my = aRb;
            battleManager.your = bRb;
            battleSpawner.hantag = transform.tag;
            battleSpawner.chotag = collision.transform.tag;
            Debug.Log("저장함1");
        }
        else
        {
            Vector3 relativeVelocity = collision.relativeVelocity;
            Vector3 impulse = collision.impulse;
            Vector3 direction = impulse.normalized;  // 내가 준 힘 방향
            Vector3 reverseDirection = -direction;   // 내가 받은 반작용 방향
            battleManager.Force = force;
            battleManager.YouForce = impulse.magnitude ;
            battleManager.YouForceDirection = direction;
            battleManager.MyForce = impulse.magnitude ;
            battleManager.MyForceDirection = reverseDirection;
            Rigidbody aRb = gameObject.GetComponent<Rigidbody>();
            Rigidbody bRb = collision.gameObject.GetComponent<Rigidbody>();
            battleManager.my = aRb;
            battleManager.your = bRb;
            battleSpawner.chotag = transform.tag;
            battleSpawner.hantag = collision.transform.tag;
            Debug.Log("저장2");
        }

        if (battleSpawner.sexton == false)
        {
            battleSpawner.stoneSpawn();
            Debug.Log("소환 호출");
            battleSpawner.sexton = true;
        }

        // 두 돌의 ViewID를 넘겨서 RPC에서 정지 처리
        int myViewID = photonView.ViewID;
        PhotonView otherView = otherRb.GetComponent<PhotonView>();
        if (otherView == null) return;

        int otherViewID = otherView.ViewID;

        //배틀 시작 함수 요청
        battleManagerView.RPC("StartBattle", RpcTarget.All);
        //배틀 스톤 소환 요청



        // 모든 클라이언트에게 요청
        photonView.RPC("PauseGame", RpcTarget.All, myViewID, otherViewID);
        battleManager.endbattle = false;
    }

    [PunRPC]
    private void PauseGame(int myViewID, int otherViewID)
    {

        // 두 돌 정지 처리
        PhotonView myView = PhotonView.Find(myViewID);
        PhotonView otherView = PhotonView.Find(otherViewID);

        if (myView != null)
        {
            Rigidbody myRb = myView.GetComponent<Rigidbody>();
            if (myRb != null)
            {
                myRb.linearVelocity = Vector3.zero;
                myRb.angularVelocity = Vector3.zero;
            }
        }

        if (otherView != null)
        {
            Rigidbody otherRb = otherView.GetComponent<Rigidbody>();
            if (otherRb != null)
            {
                otherRb.linearVelocity = Vector3.zero;
                otherRb.angularVelocity = Vector3.zero;
            }
        }
        youID = myViewID; //돌의 포톤 ID

    }

    IEnumerator turnover()
    {
        yield return new WaitForSeconds(1.5f);
        if (!battleManager.isbattle)
        {
            isShot = false;
            isDragging = false;

            

            PhotonView turnManagerView = turnManager.GetComponent<PhotonView>();
            turnManagerView.RPC("EndTurnRPC", RpcTarget.All);
        }
    }



    //[PunRPC]
    //void www(int targetViewID)
    //{
    //    Debug.Log("깔@롱");
    //    if (photonView.ViewID != targetViewID) return; // 내가 타겟이 아니면 무시
    //    else rb.AddForce(-battleManager.Force, ForceMode.Impulse);
    //}
    public IEnumerator CheckStoneStopped()
    {


        yield return new WaitForSeconds(shotCheckDelay); // 0.5초 대기
        float stoppedTime = 0f;
        float requiredStoppedDuration = 0.5f; // 0.5초 동안 멈춰있어야 진짜 멈춤으로 간주
        while (isShot)
        {
            if (battleManager.isbattle == true)
            {
                yield break;
            }
            if (rb.linearVelocity.magnitude < 0.05f && rb.angularVelocity.magnitude < 0.05f)
            {
                stoppedTime += Time.deltaTime;
                if (stoppedTime >= requiredStoppedDuration)
                {
                    isShot = false;
                    isDragging = false;
                    battleManager.usshot = false;
                    battleManager.isup = false;
                    PhotonView turnManagerView = turnManager.GetComponent<PhotonView>();
                    turnManagerView.RPC("EndTurnRPC", RpcTarget.All);
                    yield break;
                }
            }
            else
            {
                stoppedTime = 0f;
            }
            yield return null;
        }
    }

    private bool CanControl()
    {
        if (isShot) return false;
        if (TurnManager.Instance == null) return false;
        return TurnManager.Instance.CanTakeTurn();
    }

    public void ResetShotState()
    {
        isShot = false;
    }

    public void TransferOwnershipTo(Player newOwner)
    {
        isShot = false;
        if (photonView.Owner != newOwner)
        {

            photonView.TransferOwnership(newOwner);
            photonView.RPC("SetOwner", RpcTarget.AllBuffered, newOwner.ActorNumber.ToString());
        }
    }
}