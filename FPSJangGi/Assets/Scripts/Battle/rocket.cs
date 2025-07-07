
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using DamageNumbersPro;
using Unity.VisualScripting;

public class rocket : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public DamageNumber number;
    public float speed = 10f;
    public int DMG = 5;
    public GameObject particle;
    public Transform trans;
    private sklli skill;


    private bool wr =false;
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
                Debug.LogWarning("¹ß»çÀÚ PhotonView¸¦ Ã£À» ¼ö ¾ø½À´Ï´Ù.");
            }
        }
    }
    private void Start()
    {
        
        Invoke("dest", 3f);
    }

    public void dest()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    void Update()
    {
        if (skill == null)
        {
            skill = trans.GetComponent<sklli>();
        }
        // ¸ðµç Å¬¶óÀÌ¾ðÆ®°¡ ÀÚ½Å È­¸é¿¡¼­¸¸ ÃÑ¾ËÀ» ¿òÁ÷ÀÓ
        transform.position -= transform.forward * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {

        Debug.Log("ÀÞµ¿µÊ");
        //if (wr == true)
        //{
        //    if (collision.transform.tag == "hanbullet" || collision.transform.tag == "chobullet" || collision.transform.tag == "bullet")
        //    {

        //        PhotonNetwork.Destroy(gameObject);
        //    }

        //    return;
        //}
        //wr = true;


        if (collision.gameObject.CompareTag("Ground"))
        {
            if (photonView.IsMine)
            {
                GameObject boom = PhotonNetwork.Instantiate(particle.name, transform.position, Quaternion.Euler(0, 0, 0));//Ã£Àº ÇÁ¸®ÆÕ ¼îÄ­!
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else if (collision.transform.tag == "hanbullet" || collision.transform.tag == "chobullet" || collision.transform.tag == "bullet")
        {
            return;
        }
        else if (collision.transform.tag == "die")
        {
            return;
        }
        else if (collision.transform.tag != "wang" && collision.transform.tag != "sang" && collision.transform.tag != "shield" && collision.transform.tag != "hanbullet" && collision.transform.tag != "chobullet" && collision.transform.tag != "bullet")
        {

            BattleStone aa = collision.gameObject.GetComponent<BattleStone>();
            if (aa == null)
            {
                aa = collision.gameObject.GetComponent<BattleStone>();

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
            if (photonView.IsMine)
            {
                if (skill != null) skill.Hit();

                PhotonNetwork.Destroy(gameObject);
                GameObject boom = PhotonNetwork.Instantiate(particle.name, transform.position, Quaternion.Euler(0, 0, 0));//Ã£Àº ÇÁ¸®ÆÕ ¼îÄ­!

            }
        }
        else if (collision.transform.tag == "wang")
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
            if (photonView.IsMine)
            {
                if (skill != null) skill.Hit();
                PhotonNetwork.Destroy(gameObject);
                GameObject boom = PhotonNetwork.Instantiate(particle.name, transform.position, Quaternion.Euler(0, 0, 0));//Ã£Àº ÇÁ¸®ÆÕ ¼îÄ­!

            }
        }
        else if (collision.transform.tag == "sang")
        {

            battlesang aa = collision.gameObject.GetComponent<battlesang>();
            while (aa == null)
            {
                aa = collision.gameObject.GetComponent<battlesang>();

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
            if (photonView.IsMine)
            {
                if (skill != null) skill.Hit();
                PhotonNetwork.Destroy(gameObject);
                GameObject boom = PhotonNetwork.Instantiate(particle.name, transform.position, Quaternion.Euler(0, 0, 0));//Ã£Àº ÇÁ¸®ÆÕ ¼îÄ­!

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
                if (skill != null) skill.Hit();
                GameObject boom = PhotonNetwork.Instantiate(particle.name, transform.position, Quaternion.Euler(0, 0, 0));//Ã£Àº ÇÁ¸®ÆÕ ¼îÄ­!
                PhotonNetwork.Destroy(gameObject);
            }
        }
        


    }

}

