using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

using DamageNumbersPro;

public class battlesang : MonoBehaviourPun
{
    public DamageNumber number;
    public BattleSpawner battleSpawner;
    public TurnManager turnManager;
    public ResultManger resultManger;
    public BattleUi aaa;
    public float moveSpeed = 5.0f; // 이동 속도
    private Collider sex;
    private Rigidbody rb;
    private Vector3 moveDirection; // 이동 방향

    [SerializeField]
    public int HP;
    [SerializeField]
    public int Maxhp;
    [SerializeField]
    private int skillDamage;
    [SerializeField]
    private int shield;

    public GameObject bullet;
    [SerializeField]
    private Transform muzzle;
    [SerializeField]
    private ParticleSystem muzzlefire;


    public Camera cam;
    public Ccamera cem;
    public float cooldown;
    public float delay;
    private bool isfire;

    private float look;
    private float fireforword;

    public float shootY;
    public float shootX;

    public float lookpo;
    public BattleManager battleManager;
    public Animator animator;

    public int schtime = 0;
    public GameObject shild;
    public bool opensh;

    private GameObject sh;
    [SerializeField]
    private AudioSource shotsound;

    public GameObject dieob;

    private void Awake()
    {
        aaa = FindAnyObjectByType<BattleUi>();
        battleManager = FindAnyObjectByType<BattleManager>();
        battleSpawner = FindAnyObjectByType<BattleSpawner>();
        turnManager = FindAnyObjectByType<TurnManager>();
        resultManger = FindAnyObjectByType<ResultManger>();
        rb = GetComponent<Rigidbody>();
        sex = GetComponent<Collider>();

        Invoke("Ekr", 1f);
        InvokeRepeating("shil", 1f,1f);
    }

    public void Ekr()
    {
        
        if (PhotonNetwork.IsMasterClient)
        {
            cam = GameObject.FindWithTag("hancam")?.GetComponent<Camera>();
            cem = GameObject.FindWithTag("hancam")?.GetComponent<Ccamera>();

        }

        else
        {
            cam = GameObject.FindWithTag("chocam")?.GetComponent<Camera>();
            cem = GameObject.FindWithTag("hancam")?.GetComponent<Ccamera>();

        }
    }

    void shil()
    {
        if (battleManager.sex) return;
        if (!opensh)
        schtime++;
    }


    // Update is called once per frame
    void Update()
    {

        if (battleManager.sex) return;

        if (!PhotonNetwork.IsMasterClient && !this.photonView.IsMine && gameObject.layer == 8)
        {
            Debug.Log("요청했노 이기");
            this.photonView.RequestOwnership();
        }
        if (PhotonNetwork.IsMasterClient && !this.photonView.IsMine && gameObject.layer == 7)
        {
            Debug.Log("요청했노 이기");
            this.photonView.RequestOwnership();
        }


        


        if (this.photonView.IsMine)
        {
            if ( aaa.sang == null)
            {
                aaa.sang = this;
                aaa.number = 3;
            }
            if(aaa.number != 3)
            {
                aaa.number = 3;
            }
            if (schtime >= 3 && !opensh)
            {
                opensh = true;
                Debug.Log("wwwwww");
                sh = PhotonNetwork.Instantiate(shild.name, transform.position, transform.rotation, 0, new object[] { GetComponent<PhotonView>().ViewID });
                
                PhotonView shieldPV = sh.GetComponent<PhotonView>();
                shieldPV.RPC("SetParentRPC", RpcTarget.AllBuffered, photonView.ViewID);
                
                schtime = 0;

            }
            if (HP <= 0)
            {

                string mana = transform.GetComponent<Renderer>().material.name.Replace(" (Instance)", ""); ;
                Collider aa = transform.GetComponent<Collider>();
                aa.enabled = false;
                GameObject buk = PhotonNetwork.Instantiate("die/" + dieob.name, transform.position, transform.rotation, 0, new object[] { mana });

                PhotonView arr = resultManger.GetComponent<PhotonView>();
                if(sh != null)PhotonNetwork.Destroy(sh);
                
                arr.RPC("endbattle", RpcTarget.All, gameObject.layer);

                PhotonNetwork.Destroy(gameObject);
            }
            if (PhotonNetwork.IsMasterClient)
            {

                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");
                transform.position += new Vector3(verticalInput * moveSpeed * Time.deltaTime, 0, -horizontalInput * moveSpeed * Time.deltaTime);

                RotateToMouse();
            }
            else
            {

                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");
                transform.position += new Vector3(-verticalInput * moveSpeed * Time.deltaTime, 0, horizontalInput * moveSpeed * Time.deltaTime);

                RotateToMouse();
            }


        }
        if (this.photonView.IsMine)
        {
            
                if (Input.GetMouseButtonDown(0) && !isfire)
                {

                    StartCoroutine(Fire1());
                    //// 현재 오브젝트의 회전 값을 받아옴
                    //float currentZ = transform.eulerAngles.y;

                    //// 회전값 지정: X=0, Y=현재 Y축 값, Z=90
                    //Quaternion rot = Quaternion.Euler(0f, currentZ + 159.344f, 90f);

                    //GameObject bulletObj = PhotonNetwork.Instantiate(bullet.name, muzzle.position, rot);

                    //bulletObj.tag = "hanbullet";
                }
            

        }


        




    }

    IEnumerator Fire1()
    {
        isfire = true;
        // 현재 오브젝트의 회전 값을 받아옴
        float currentZ = transform.eulerAngles.y;

        // 회전값 지정: X=0, Y=현재 Y축 값, Z=90
        Quaternion rot = Quaternion.Euler(0f + shootX, currentZ + shootY, 90f);

        if (muzzlefire != null) muzzlefire.Play();
        GameObject bulletObj = PhotonNetwork.Instantiate(bullet.name, muzzle.position, rot, 0, new object[] { GetComponent<PhotonView>().ViewID });
        shotsound.Play();




        //PhotonNetwork.Instantiate(bullet.name, muzzle.position, rot);
        //PhotonNetwork.Instantiate(bullet.name, muzzle.position, rot);
        //PhotonNetwork.Instantiate(bullet.name, muzzle.position, rot);
        //PhotonNetwork.Instantiate(bullet.name, muzzle.position, rot);



        photonView.RPC("anim", RpcTarget.All);
        cooldown = delay;
        StartCoroutine(attectcool());
        yield break;
    }


    [PunRPC]

    public void anim()
    {
        animator.SetTrigger("aaa");
    }
    private void RotateToMouse()
    {
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position); // y=0 평면

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 dir = hitPoint - transform.position;
            dir.y = 0f; // 평면상 방향만 사용

            float yAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            // 기존 X -90 유지, Z 0 유지, Y만 회전
            transform.rotation = Quaternion.Euler(-90f, yAngle + lookpo, 0f);






        }
    }
    IEnumerator attectcool()
    {
        while (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
            yield return null;
        }
        isfire = false; // 쿨타임 끝남
    }

    [PunRPC]

    public void Dameged(int dmg)
    {
        
        HP -= dmg;
        schtime = 0;
    }
    [PunRPC]
    public void dokkislow(int dmg)
    {
        
        HP -= dmg;
        schtime = 0;
        StartCoroutine(slow());
    }

    IEnumerator slow()
    {
        moveSpeed -= 0.5f;
        yield return new WaitForSeconds(1);
        moveSpeed += 0.5f;
    }

    [PunRPC]
    public void delete()
    {
        if(sh != null)PhotonNetwork.Destroy(sh);
        
        PhotonNetwork.Destroy(gameObject);
    }
}

