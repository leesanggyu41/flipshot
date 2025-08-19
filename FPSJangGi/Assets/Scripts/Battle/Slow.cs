using UnityEngine;
using Photon.Pun;
using DamageNumbersPro;
using Unity.VisualScripting;

public class Slow : MonoBehaviourPun
{
    public DamageNumber number;
    public float speed = 10f;
    public int DMG = 5;
    


    private void Start()
    {
        
        Invoke("dest", 1.5f);
    }

    public void dest()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        

        // 모든 클라이언트가 자신 화면에서만 총알을 움직임
        transform.position += transform.up * speed * Time.deltaTime;

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == transform.tag) return;


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
                rr.RPC("dokkislow", RpcTarget.Others, (float)DMG * 0.7f);
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
                rr.RPC("dokkislow", RpcTarget.Others, (float)DMG);
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
            rr.RPC("dokkislow", RpcTarget.Others, DMG);
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
                rr = collision.transform.GetComponent<shld>();
            }
            PhotonView aa = rr.transform.GetComponent<PhotonView>();
            aa.RPC("Dameged", RpcTarget.Others, DMG);
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
                
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else if (collision.transform.tag == "hanbullet" || collision.transform.tag == "chobullet" || collision.transform.tag == "bullet" || collision.transform.tag== "die")
        {

            return;
        }
        
        else
        {

            BattleStone aa = collision.gameObject.GetComponent<BattleStone>();
            while (aa == null)
            {
                aa = collision.gameObject.GetComponent<BattleStone>();

            }
            PhotonView rr = aa.gameObject.GetComponent<PhotonView>();
            DamageNumber yas = number.Spawn(gameObject.transform.position, DMG);

            if (PhotonNetwork.IsMasterClient)
            {
                yas.gameObject.transform.Rotate(90, 0, -90);
            }
            else
            {
                yas.gameObject.transform.Rotate(90, 180, -90);
            }
            rr.RPC("dokkislow", RpcTarget.Others, DMG);
            
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }


    }
    

}


