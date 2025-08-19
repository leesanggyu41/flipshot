using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;
using System.Collections;

public class sklli : MonoBehaviourPun
{

    public bool hit = false;
    public int time = 0;
    public BattleStone battleStone;
    public battlesang Basang;
    public BattleUi aaa;
    public GameObject sklliob;
    public Transform muzzle;
    public int delay;
    private bool ch;
    public float cooldownTimer = 0f; 
    public float maxCooldown = 0f;

    [SerializeField]
    private int maxhp;
    [SerializeField]
    private AudioSource speedup;
    [SerializeField]
    private AudioSource skillsouind; 

    private void Awake()
    {
        aaa = FindAnyObjectByType<BattleUi>();
    }

    
        
        
    
    void Update()
    {
        if (this.photonView.IsMine)
        {
            if (aaa.skillScript == null) aaa.skillScript = this;
            if (Input.GetMouseButtonDown(1) && !ch)
            {
                if(transform.tag == "zzol")
                {
                    if(PhotonNetwork.IsMasterClient)
                    StartCoroutine(Skill1());
                    else
                    {
                    StartCoroutine(Skill2());
                    }
                }
                else if(transform.tag == "po")
                {
                    StartCoroutine(poskill());
                }
                else if (transform.tag == "sang")
                {
                    StartCoroutine(sangskill());
                }
                else if(transform.tag == "cha")
                {
                    StartCoroutine(chaskill());
                }
                else if(transform.tag == "ma")
                {
                    if (PhotonNetwork.IsMasterClient)
                        StartCoroutine(Skill1());
                    else
                    {
                        StartCoroutine(Skill2());
                    }
                }
                else if (transform.tag == "sa")
                {
                    saskill();
                }

            
            }


            //패시브 스킬
            if (transform.tag == "zzol" && hit || transform.tag == "sa" && hit)
            {
                battleStone.moveSpeed = 4;
            }
            

            
        }


        

        
    }
    
    IEnumerator Skill1()
    {
        ch = true;
        float currentZ = transform.eulerAngles.y;
        
            Quaternion rot = Quaternion.Euler(0f + battleStone.shootX, currentZ + battleStone.shootY, 90f);
            GameObject skiObj = PhotonNetwork.Instantiate(sklliob.name, muzzle.position, rot);
            skiObj.transform.tag = "hanbullet";
            skiObj.layer = 10;
        if(skillsouind != null)skillsouind.Play();



        maxCooldown = delay;
        cooldownTimer = delay;
        StartCoroutine(CooldownTimer());
        yield break;
    }
    IEnumerator Skill2()
    {
        ch = true;
        float currentZ = transform.eulerAngles.y;
        
            Quaternion rot = Quaternion.Euler(0f + battleStone.shootX, currentZ + battleStone.shootY, 90f);
            GameObject skiObj = PhotonNetwork.Instantiate(sklliob.name, muzzle.position, rot);
            skiObj.transform.tag = "chobullet";
        if (skillsouind != null) skillsouind.Play();
        skiObj.layer = 11;
        maxCooldown = delay;
        cooldownTimer = delay;
        StartCoroutine(CooldownTimer());
        yield break;
    }

    IEnumerator poskill()
    {
        ch = true;
        battleStone.delay = 0.5f;

        skillsouind.Play();
        maxCooldown = delay;
        cooldownTimer = delay;
        StartCoroutine(CooldownTimer());
        yield return new WaitForSeconds(3);
        battleStone.delay = 2;
        
        
    }

    IEnumerator sangskill()
    {
        ch = true;
        Basang.moveSpeed = 2.5f;
        skillsouind.Play();
        maxCooldown = delay;
        cooldownTimer = delay;
        StartCoroutine(CooldownTimer());
        yield return new WaitForSeconds(3);
        Basang.moveSpeed = 1.5f;
    }

    IEnumerator chaskill()
    {
        ch = true;
        battleStone.buf = true;

        skillsouind.Play();
        maxCooldown = delay;
        cooldownTimer = delay;
        StartCoroutine(CooldownTimer());
        yield return new WaitForSeconds(3);
        battleStone.buf = false;
    }
    
    void saskill()
    {
        ch = true;
        PhotonNetwork.Instantiate(sklliob.name, transform.position, transform.rotation);
        photonView.RPC("heal", RpcTarget.All);
        skillsouind.Play();
        maxCooldown = delay;
        cooldownTimer = delay;
        StartCoroutine(CooldownTimer());
        
    }
    [PunRPC]
    void heal()
    {
        battleStone.HP += 30;
        if (battleStone.HP > maxhp) battleStone.HP = maxhp;
    }
    IEnumerator CooldownTimer()
    {
        while (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }
        ch = false; // 쿨타임 끝남
    }

    public void Hit()
    {
        if(transform.tag == "zzol" || transform.tag == "sa")
        {
            time = 1;
            
        StopCoroutine(timeto());
        StartCoroutine(timeto());
        }
        else if (transform.tag == "po")
        {
            if (ch && cooldownTimer > 0f)
            {
                float reduceAmount = 1f;
                cooldownTimer -= reduceAmount;

                if (cooldownTimer < 0f)
                    cooldownTimer = 0f;

                
            }
        }
        else
        {
            return;
        }
        

    }

    IEnumerator timeto()
    {
        hit = true;
        if (speedup != null && battleStone.moveSpeed == 2.5f)speedup.Play();
        aaa.passiveActive = true;
        yield return new WaitForSeconds(time);
        hit = false;
        aaa.passiveActive = false;
        battleStone.moveSpeed = 2.5f;
    }
    
 }
