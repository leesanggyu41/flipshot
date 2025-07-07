using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

using Photon.Pun.Demo.Asteroids;
using DamageNumbersPro;
using Unity.VisualScripting;

public class BattleStone : MonoBehaviourPun
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


    public float cooldown;




    public GameObject bullet;
    public GameObject bullet1;
    [SerializeField]
    private Transform muzzle;
    [SerializeField]
    private ParticleSystem muzzlefire;


    public Camera cam;
    public Ccamera cem;

    public float delay;
    private bool isfire;
    public bool buf;

    private float look;
    private float fireforword;

    public float shootY;
    public float shootX;

    public float lookpo;
    public BattleManager battleManager;
    [SerializeField]
    private int count = 0;

    private Bullet bllet;
    private rocket Rocket;

    [SerializeField]
    private AudioSource[] shotsound;

    public GameObject dieob;

    private void Awake()
    {
        aaa = FindAnyObjectByType<BattleUi>();
        battleSpawner = FindAnyObjectByType<BattleSpawner>();
        turnManager = FindAnyObjectByType<TurnManager>();
        resultManger = FindAnyObjectByType<ResultManger>();
        battleManager = FindAnyObjectByType<BattleManager>();
        rb = GetComponent<Rigidbody>();
        sex = GetComponent<Collider>();

        Invoke("Ekr", 1f);
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



    // Update is called once per frame
    void Update()
    {
        
        if (battleManager.sex) return;
        
        if (!PhotonNetwork.IsMasterClient && !this.photonView.IsMine && gameObject.layer == 8)
        {
            Debug.Log("요청했노 이기");
            this.photonView.RequestOwnership();
        }
        if(PhotonNetwork.IsMasterClient && !this.photonView.IsMine && gameObject.layer == 7)
        {
            Debug.Log("요청했노 이기");
            this.photonView.RequestOwnership();
        }

        if(transform.tag == "ma"&& count >= 2)
        {
            aaa.passiveActive = true;
        }
        else if (transform.tag == "ma" && count <= 2)
        {
            aaa.passiveActive = false;
        }

        if (photonView.IsMine && aaa.battleStone == null)
        {
            aaa.battleStone = this;
            aaa.number = 1;

            if (transform.tag == "cha" || transform.tag == "po") aaa.passiveActive = true;
        }
        if (this.photonView.IsMine)
        {
            if (aaa.number != 1)
            {
                aaa.number = 1;
            }

            if (HP <= 0)
            {
                string mana = transform.GetComponent<Renderer>().material.name.Replace(" (Instance)", ""); ;
                Collider aa = transform.GetComponent<Collider>();
                aa.enabled = false;
                GameObject buk = PhotonNetwork.Instantiate("die/" + dieob.name, transform.position, transform.rotation, 0, new object[] { mana });
                
                PhotonView arr = resultManger.GetComponent<PhotonView>();

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
            if (PhotonNetwork.IsMasterClient)
            {
                if (Input.GetMouseButtonDown(0) && !isfire)
                {

                    StartCoroutine(Fire1()); 
                    
                }
            }
            else if (!PhotonNetwork.IsMasterClient)
            {
                if (Input.GetMouseButtonDown(0) && !isfire)
                {

                    StartCoroutine(Fire2());
                    
                }
            }

        }


        




    }

    IEnumerator Fire1()
    {
        isfire = true;
        // 현재 오브젝트의 회전 값을 받아옴
        float currentZ = transform.eulerAngles.y;

        // 회전값 지정: X=0, Y=현재 Y축 값, Z=90
        Quaternion rot = Quaternion.Euler(0f + shootX, currentZ + shootY, 90f );
        if(muzzlefire !=null)muzzlefire.Play();
        if (transform.tag == "ma" && count >= 2 || transform.tag == "cha" && buf)
        {
            GameObject bulletObj1 = PhotonNetwork.Instantiate(bullet1.name, muzzle.position, rot, 0, new object[] { GetComponent<PhotonView>().ViewID });
            shotsound[1].Play();


            
            count = 0;
            
            bulletObj1.tag = "hanbullet";
            bulletObj1.layer = 10;
        }
        else
        {
            GameObject bulletObj = PhotonNetwork.Instantiate(bullet.name, muzzle.position, rot, 0, new object[] { GetComponent<PhotonView>().ViewID });
            shotsound[0].Play();

            if (transform.tag == "ma") count++;
            bulletObj.tag = "hanbullet";
            bulletObj.layer = 10;
        }



        cooldown = delay;
        StartCoroutine(attectcool());
        yield break;
        
        
    }

    IEnumerator Fire2()
    {
        isfire = true;
        // 현재 오브젝트의 회전 값을 받아옴
        float currentZ = transform.eulerAngles.y;

        // 회전값 지정: X=0, Y=현재 Y축 값, Z=90
        Quaternion rot = Quaternion.Euler(0f + shootX, currentZ + shootY, 90f );
        if (muzzlefire != null) muzzlefire.Play();
        if (transform.tag == "ma" && count >= 2 || transform.tag == "cha" && buf)
        {
            GameObject bulletObj1 = PhotonNetwork.Instantiate(bullet1.name, muzzle.position, rot ,0, new object[] { GetComponent<PhotonView>().ViewID });
            shotsound[1].Play();



            count = 0;
            

            bulletObj1.tag = "chobullet";
            bulletObj1.layer = 11;
        }
        else
        {
            GameObject bulletObj = PhotonNetwork.Instantiate(bullet.name, muzzle.position, rot, 0,new object[] { GetComponent<PhotonView>().ViewID });
            shotsound[0].Play();
            if (transform.tag == "ma") count++;
            bulletObj.tag = "chobullet";
            bulletObj.layer = 11;
        }
        cooldown = delay;
        StartCoroutine(attectcool());
        yield break;
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

        photonView.RPC("hpview", RpcTarget.Others, HP);
    }
    [PunRPC]
    void hpview(int hp)
    {
        HP = hp;
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.transform.tag =="hanbullet" || collision.transform.tag == "chobullet")
    //    {
    //        int DMG = collision.transform.GetComponent<Bullet>().DMG;
    //        number.Spawn(gameObject.transform.position, DMG);
    //    }
    //}




    [PunRPC]
    public void dokkislow(int dmg)
    {
        
        Debug.Log("데미지띄우기");
        HP -= dmg;
        moveSpeed -= 0.5f;
        StartCoroutine(slow());

    }

    IEnumerator slow()
    {

        
        yield return new WaitForSeconds(2);
        
        moveSpeed += 0.5f;

    }


    [PunRPC]
    public void delete()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}

