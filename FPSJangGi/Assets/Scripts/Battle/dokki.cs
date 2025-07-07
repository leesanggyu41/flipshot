using UnityEngine;
using Photon.Pun;
using DamageNumbersPro;
using Unity.VisualScripting;


public class dokki : MonoBehaviourPun
{
    [SerializeField]
    private float Speed;

    public int DMG;

    private Rigidbody rb;
    public float throwForce = 10f;
    public float rotationSpeed = 20f;

    public Collider nu;
    public DamageNumber number;

    void Start()
    {
        nu = GetComponent<Collider>();
        Invoke("aaa", 0.2f);
        rb = GetComponent<Rigidbody>();
        Invoke("des", 1);
        rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        rb.AddTorque(transform.right * rotationSpeed, ForceMode.Impulse);
    }

    
    public void aaa()
    {
        nu.enabled = true;
    }

    public void des()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            return;
        }
        else if (other.tag == "hanbullet" || other.tag == "chobullet" || other.tag == "bullet" || other.tag =="die")
        {

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
        }
        else if (other.tag == "sang")
        {
            
            battlesang aa = other.gameObject.GetComponent<battlesang>();
            while (aa == null)
            {
                aa = other.gameObject.GetComponent<battlesang>();

            }
            PhotonView rr = aa.gameObject.GetComponent<PhotonView>();
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
        else if (other.tag != "sang" || other.tag != "wang" || other.tag != "Ground")
        {
            
            BattleStone aa = other.gameObject.GetComponent<BattleStone>();
            while (aa == null)
            {
                aa = other.gameObject.GetComponent<BattleStone>();

            }
            PhotonView rr = aa.gameObject.GetComponent<PhotonView>();
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
        else if(other.tag == "shield")
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

            if (photonView.IsMine)
            {
                
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    

}
