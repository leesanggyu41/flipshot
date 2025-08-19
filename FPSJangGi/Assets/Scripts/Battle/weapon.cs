using UnityEngine;
using Photon.Pun;
using DamageNumbersPro;
using Unity.VisualScripting;

public class weapon : MonoBehaviourPun
{
    public int DMG;
    public DamageNumber number;
    public Transform nal;
    public Collider myCollider;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Ground")
        {
            return;
        }
        else if (collision.transform.tag == "hanbullet" || collision.transform.tag == "chobullet" || collision.transform.tag == "bullet")
        {
            return;
        }
        else if(collision.tag == "die")
        {
            return;
        }
        if (collision.transform.CompareTag("wang"))
        {

            goung aa = collision.gameObject.GetComponent<goung>();
            while (aa == null)
            {
                aa = collision.gameObject.GetComponent<goung>();

            }
            PhotonView rr = aa.gameObject.GetComponent<PhotonView>();
            if (aa.HP <= 50)
            {
                rr.RPC("Dameged", RpcTarget.Others, (float)DMG *0.7f);
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
            
            myCollider.enabled = false;
            return;
        }
        else if(collision.tag == "sang")
        {

            battlesang aa = collision.gameObject.GetComponent<battlesang>();
            while (aa == null)
            {
                aa = collision.gameObject.GetComponent<battlesang>();

            }
            
            PhotonView rr = aa.gameObject.GetComponent<PhotonView>();
            DamageNumber yas = number.Spawn(nal.position, DMG);

            if (PhotonNetwork.IsMasterClient)
            {
                yas.gameObject.transform.Rotate(90, 0, -90);
            }
            else
            {
                yas.gameObject.transform.Rotate(90, 180, -90);
            }
            rr.RPC("Dameged", RpcTarget.Others, DMG);
            myCollider.enabled = false;
            return;
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
            DamageNumber yas = number.Spawn(nal.position, DMG);

            if (PhotonNetwork.IsMasterClient)
            {
                yas.gameObject.transform.Rotate(90, 0, -90);
            }
            else
            {
                yas.gameObject.transform.Rotate(90, 180, -90);
            }
            myCollider.enabled = false;
            return;
        }
        else if (collision.transform.tag != "Ground")
        {

            BattleStone aa = collision.gameObject.GetComponent<BattleStone>();
            while (aa == null)
            {
                aa = collision.gameObject.GetComponent<BattleStone>();

            }
            PhotonView rr = aa.gameObject.GetComponent<PhotonView>();
            DamageNumber yas = number.Spawn(nal.position, DMG);

            if (PhotonNetwork.IsMasterClient)
            {
                yas.gameObject.transform.Rotate(90, 0, -90);
            }
            else
            {
                yas.gameObject.transform.Rotate(90, 180, -90);
            }
            rr.RPC("Dameged", RpcTarget.Others, DMG);
            myCollider.enabled = false;
            return;
        }
    }

    
}
