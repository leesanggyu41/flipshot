using UnityEngine;
using System.Collections;
using Photon.Pun;
using DamageNumbersPro;
using Unity.VisualScripting;


public class stap : MonoBehaviourPun
{
    public DamageNumber number;
    public int Speed;
    public int DMG = 30;
    
    void Start()
    {
        Invoke("des", 0.1f);
    }
    private void Update()
    {
        transform.position += transform.up * Speed * Time.deltaTime;
    }

    void des()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Ground")
        {
            return;
        }
        else if (other.tag == "hanbullet" || other.tag == "chobullet" || other.tag == "bullet")
        {

            return;
        }
        else if(other.tag == "die"){
            return;
        }
        if (other.tag == "wang")
        {
            
            goung aa = other.gameObject.GetComponent<goung>();
            while (aa == null)
            {
                aa = other.gameObject.GetComponent<goung>();

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

            rr.RPC("Dameged", RpcTarget.Others, (float)DMG);

            return;
        }
        else if (other.tag == "shield")
        {
            shld rr = other.transform.GetComponent<shld>();
            while (rr == null)
            {
                rr = other.transform.GetComponent<shld>();
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

            return;
        }
        else if(other.tag == "sang")
        {
            
            battlesang aa = other.gameObject.GetComponent<battlesang>();
            while (aa == null)
            {
                aa = other.gameObject.GetComponent<battlesang>();

            }
            PhotonView rr = aa.gameObject.GetComponent<PhotonView>();
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
            return;
        }
        else if (other.tag != "sang" || other.tag != "wang" || other.tag != "ground")
        {
            
            BattleStone aa = other.gameObject.GetComponent<BattleStone>();
            while (aa == null)
            {
                aa = other.gameObject.GetComponent<BattleStone>();

            }
            PhotonView rr = aa.gameObject.GetComponent<PhotonView>();
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
            return;
        }
    }

}
