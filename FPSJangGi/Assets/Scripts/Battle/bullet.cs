using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using DamageNumbersPro;
using NUnit.Framework;

public class Bullet : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public DamageNumber number;
    public float speed = 10f;
    public int DMG = 5;
    public Transform trans;
    private sklli skill;
    [SerializeField]
    private int ta;
    [SerializeField]
    private TrailRenderer GGori;

    [SerializeField]
    private int cri;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = photonView.InstantiationData;
        if (data != null && data.Length > 0)
        {
            int shooterViewID = (int)data[0];
            PhotonView shooterPV = PhotonView.Find(shooterViewID);

            if (shooterPV != null)
            {
                trans = shooterPV.transform;
                
            }
            else
            {
                Debug.LogWarning("발사자 PhotonView를 찾을 수 없습니다.");
            }
        }
    }
    private void Start()
    {

        
        Invoke("dest", 1.5f);
        if (ta == 1)
        {
            int r =Random.Range(0, cri);
            if (r == 1 || r == 2)
            {
                DMG = 40;
                GGori.startColor = Color.red;
                GGori.endColor = Color.yellow;
            }
        }
    }


    public void SetShooter(Transform t)
    {
        trans = t;
    }
    public void dest()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if(skill == null)
        {
        skill = trans.GetComponent<sklli>();
        }

        // 모든 클라이언트가 자신 화면에서만 총알을 움직임
        transform.position += transform.up * speed * Time.deltaTime;
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == transform.tag)return;
        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point;
        Vector3 hitNormal = contact.normal;

        Quaternion effectRotation = Quaternion.LookRotation(hitNormal); 

        GameObject clone = PhotonNetwork.Instantiate("effectt", hitPoint, effectRotation);

        StartCoroutine(destroyeffect(clone));

        if (collision.transform.CompareTag("Ground"))
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else if (collision.transform.CompareTag("wang"))
        {
            
            
            goung aa = collision.transform.GetComponent<goung>();
            while (aa == null)
            {
                 aa = collision.transform.GetComponent<goung>();
                 
            }
            PhotonView rr = aa.transform.GetComponent<PhotonView>();
            

            if (aa.HP <= 50)
            {
                rr.RPC("Dameged", RpcTarget.Others, (float)DMG * 0.7f);
                DamageNumber yas = number.Spawn(gameObject.transform.position, (int)DMG * 0.7f);
                if (PhotonNetwork.IsMasterClient)
                {
                    yas.gameObject.transform.Rotate(90, 0, -90);
                }
                else
                {
                    yas.gameObject.transform.Rotate(90, 180, -90);
                }
            }
            else
            {
                rr.RPC("Dameged", RpcTarget.Others, (float)DMG);
                DamageNumber yas = number.Spawn(gameObject.transform.position, DMG);
                if (PhotonNetwork.IsMasterClient)
                {
                    yas.gameObject.transform.Rotate(90, 0, -90);
                }
                else
                {
                    yas.gameObject.transform.Rotate(90, 180, -90);
                }
            }



            if (photonView.IsMine)
            {
                if (skill != null) skill.Hit();
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else if (collision.transform.tag == "sang")
        {
            
            
            battlesang aa = collision.transform.GetComponent<battlesang>();
            while (aa == null)
            {
                aa = collision.transform.GetComponent<battlesang>();

            }
            PhotonView rr = aa.transform.GetComponent<PhotonView>();

            rr.RPC("Dameged", RpcTarget.Others, DMG);
            DamageNumber yas = number.Spawn(gameObject.transform.position, DMG);

            if (PhotonNetwork.IsMasterClient)
            {
                yas.gameObject.transform.Rotate(90, 0, -90);
            }
            else
            {
                yas.gameObject.transform.Rotate(90, 180, -90);
            }
        }
        else if (collision.transform.tag == "shield")
        {
            shld rr = collision.transform.GetComponent<shld>();
            while (rr == null)
            {
                Debug.Log("ekrsss");
                rr = collision.transform.GetComponent<shld>();
            }
            PhotonView aa = rr.transform.GetComponent<PhotonView>();

            aa.RPC("Damegede", RpcTarget.Others, DMG);
            DamageNumber yas = number.Spawn(gameObject.transform.position, DMG);

            if (PhotonNetwork.IsMasterClient)
            {
                yas.gameObject.transform.Rotate(90, 0, -90);
            }
            else
            {
                yas.gameObject.transform.Rotate(90, 180, -90);
            }

            if (photonView.IsMine)
            {
                if (skill != null) skill.Hit();
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else if (collision.transform.tag == "hanbullet" || collision.transform.tag == "chobullet" || collision.transform.tag == "bullet" || collision.transform.tag == "die")
        {

            return;
        }
        else
        {

            BattleStone aa = collision.transform.GetComponent<BattleStone>();
            while (aa == null)
            {
                aa = collision.transform.GetComponent<BattleStone>();

            }
            PhotonView rr = aa.transform.GetComponent<PhotonView>();
            rr.RPC("Dameged", RpcTarget.Others, DMG);
            DamageNumber yas = number.Spawn(gameObject.transform.position, DMG);

            if (PhotonNetwork.IsMasterClient)
            {
                yas.gameObject.transform.Rotate(90, 0, -90);
            }
            else
            {
                yas.gameObject.transform.Rotate(90, 180, -90);
            }






            if (photonView.IsMine)
            {
                if (skill != null) skill.Hit();
                PhotonNetwork.Destroy(gameObject);
            }
        }


    }

    

    IEnumerator destroyeffect(GameObject aa)
    {
        yield return new WaitForSeconds(2);
        Destroy(aa);
    }
}

