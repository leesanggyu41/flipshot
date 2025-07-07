using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;

public class shld : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public BattleUi aaa;
    public DamageNumber number;
    public int HP;
    public battlesang mast;
    public Transform trans;
    [SerializeField] private Material sui;
    [SerializeField] private Collider Cd;
    [SerializeField] private Collider parCd;

    [SerializeField] private AudioSource sw;
    [SerializeField] private AudioSource sh;

    private bool dir;

    

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
                Debug.Log("�߻��� ���� �Ϸ�: " + trans.name);
            }
            else
            {
                Debug.LogWarning("�߻��� PhotonView�� ã�� �� �����ϴ�.");
            }
        }
    }

    

    [PunRPC]
    public void SetParentRPC(int parentViewID)
    {
        PhotonView parentPV = PhotonView.Find(parentViewID);
        if (parentPV != null)
        {
            transform.SetParent(parentPV.transform);
            Debug.Log("�θ� ���� �Ϸ�: " + parentPV.name);
        }
        else
        {
            Debug.LogWarning("�θ� ã�� �� �����ϴ�.");
        }
        aaa = FindAnyObjectByType<BattleUi>();
        photonView.RPC("spawn", RpcTarget.All);
    }

    [PunRPC]
    private void spawn()
    {
        aaa.passiveActive = true;
        parCd = trans.GetComponent<Collider>();
        Cd.enabled = false;
        sw.Play();
        StartCoroutine(sung());
    }

    IEnumerator sung()
    {
        for (float i = 0; i < 1; i += 0.01f)
        {
            sui.SetFloat("_fateValue", i);
            yield return null;

        }

        Cd.enabled = true;
        
    }
    private void Update()
    {

        
        if(mast == null)
        {
            mast = transform.parent.GetComponent<battlesang>();
        }
        if (HP <= 0 && !dir)
        {
            dir = true;
            photonView.RPC("die", RpcTarget.All);
            
        }
    }
    [PunRPC]
    public void Damegede(int dmg)
    {
        Debug.Log("��ȣ�� ����");
        Debug.Log(dmg);
        HP -= dmg;
        
        Debug.Log(dmg + "������  ����ü��" + HP );
    }

    [PunRPC]
    public void die()
    {
        aaa.passiveActive = false;
        Cd.enabled = false;
        sh.Play();
        StartCoroutine(breaking());
    }


    IEnumerator breaking()
    {
        for (float i = 1; i >= 0; i -= 0.01f)
        {
            sui.SetFloat("_fateValue", i);
            yield return null;

        }
        mast.opensh = false;
        
        PhotonNetwork.Destroy(gameObject);
    }
}
