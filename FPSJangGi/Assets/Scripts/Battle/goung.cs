using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

using static UnityEngine.ParticleSystem;
using Photon.Pun.Demo.Asteroids;
using Unity.VisualScripting;
using DamageNumbersPro;
using Unity.Jobs;

public class goung : MonoBehaviourPun
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


    public float cooldown;

    public Camera cam;
    public Ccamera cem;

    public float delay;
    private bool isfire;

    private float look;
    private float fireforword;

    public float shootY;
    public float shootX;

    private int type = 1;

    public Animator Attect;
    [SerializeField]
    private Collider weapon1;
    [SerializeField]
    private Collider weapon2;

    public TrailRenderer particle1;

    public float ypos;
    public bool isSkill = false;
    public GameObject dokkis;
    public Transform muzzle;
    public int skillcol;
    public BattleManager battleManager;

    public float cooldownTimer = 0f;
    private float maxCooldown = 0f;

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
        if (PhotonNetwork.IsMasterClient && !this.photonView.IsMine && gameObject.layer == 7)
        {
            Debug.Log("요청했노 이기");
            this.photonView.RequestOwnership();
        }
        


        if (this.photonView.IsMine)
        {
            if (aaa.wang == null)
            {
                aaa.wang = this;
                aaa.number = 2;
            }
            if (aaa.number != 2)
            {
                aaa.number = 2;
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
            if (type == 1)
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
            else if (type == 2)
            {
                if (Input.GetMouseButtonDown(0) && !isfire)
                {
                    Debug.Log("ww");
                    StartCoroutine(Fire2());
                    //// 현재 오브젝트의 회전 값을 받아옴
                    //float currentZ = transform.eulerAngles.y;

                    //// 회전값 지정: X=0, Y=현재 Y축 값, Z=90
                    //Quaternion rot = Quaternion.Euler(0f, currentZ + 159.344f, 90f);

                    //GameObject bulletObj = PhotonNetwork.Instantiate(bullet.name, muzzle.position, rot);
                    //bulletObj.tag = "chobullet";
                }
            }
            if (Input.GetMouseButtonDown(1) && !isSkill)
            {
                if(PhotonNetwork.IsMasterClient)
                StartCoroutine(Dash());
                else
                StartCoroutine(Dash1());
            }
            
        }
        

        
         
        
    }

    IEnumerator Fire1()
    {
        weapon1.enabled = true;
        weapon2.enabled = true;
        isfire = true;
        photonView.RPC("ani1", RpcTarget.All);
        shotsound.Play();
        type = 2;
        
        //ypos = transform.rotation.z;
        //Debug.Log(ypos);
        // GameObject boom = PhotonNetwork.Instantiate(particle1.name, transform.position, Quaternion.Euler(-90, ypos, 0));//찾은 프리팹 쇼칸!
        cooldown = delay;
        StartCoroutine(attectcool());
        yield break;
        
        
    }
    
    IEnumerator Fire2()
    {
        weapon1.enabled = true;
        weapon2.enabled = true;
        isfire = true;
        photonView.RPC("ani2", RpcTarget.All);
        shotsound.Play();
        type = 1;
        
        //ypos = transform.rotation.z;
        //Debug.Log(ypos);
        //GameObject boom = PhotonNetwork.Instantiate(particle1.name, transform.position, Quaternion.Euler(90, ypos, 180));//찾은 프리팹 쇼칸!
        cooldown = delay;
        StartCoroutine(attectcool());
        yield break;
        
        
    }

    [PunRPC]
    void ani1()
    {
        Attect.SetInteger("type", 1);
        particle1.enabled = true;
    }
    [PunRPC]
    void ani2()
    {
        Attect.SetInteger("type", 2);
        particle1.enabled = true;
    }

    IEnumerator Dash()
    {
        isSkill = true;
        // 현재 오브젝트의 회전 값을 받아옴
        float currentZ = transform.eulerAngles.y;

        // 회전값 지정: X=0, Y=현재 Y축 값, Z=90
        Quaternion rot = Quaternion.Euler(0f + shootX, currentZ + shootY, 90f);

        GameObject bulletObj = PhotonNetwork.Instantiate(dokkis.name, muzzle.position, rot);

        bulletObj.tag = "hanbullet";
        maxCooldown = skillcol;
        cooldownTimer = skillcol;
        StartCoroutine(CooldownTimer());
        yield break;

    }
    IEnumerator Dash1()
    {
        isSkill = true;
        // 현재 오브젝트의 회전 값을 받아옴
        float currentZ = transform.eulerAngles.y;

        // 회전값 지정: X=0, Y=현재 Y축 값, Z=90
        Quaternion rot = Quaternion.Euler(0f + shootX, currentZ + shootY, 90f);

        GameObject bulletObj = PhotonNetwork.Instantiate(dokkis.name, muzzle.position, rot);

        bulletObj.tag = "chobullet";
        maxCooldown = skillcol;
        cooldownTimer = skillcol;
        StartCoroutine(CooldownTimer());
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
                transform.rotation = Quaternion.Euler(-90f, yAngle - 73f, 0f);
            
            





        }
    }
    IEnumerator attectcool()
    {
        while (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
            yield return null;
        }
        particle1.enabled = false;
        weapon1.enabled = false;
        weapon2.enabled = false;
        isfire = false;
    }

    IEnumerator CooldownTimer()
    {
        while (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }
        isSkill = false; // 쿨타임 끝남
    }

    [PunRPC]

    public void Dameged(float dmg)
    {
        
            HP -= (int)dmg;
        photonView.RPC("hpview", RpcTarget.Others, HP);


     }
    [PunRPC]
    public void hpview(int hp)
    {
        HP = hp;
    }

    [PunRPC]
    public void dokkislow(int dmg)
    {
        HP -= dmg;
        StartCoroutine(slow());
        
    }

    IEnumerator slow()
    {
        moveSpeed -= 0.5f;
        yield return new WaitForSeconds(2);
        moveSpeed += 0.5f;
    }

    [PunRPC]
    public void delete()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}

