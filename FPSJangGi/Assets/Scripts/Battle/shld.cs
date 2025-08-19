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
                Debug.Log("발사자 연결 완료: " + trans.name);
            }
            else
            {
                Debug.LogWarning("발사자 PhotonView를 찾을 수 없습니다.");
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
            Debug.Log("부모 설정 완료: " + parentPV.name);
        }
        else
        {
            Debug.LogWarning("부모를 찾을 수 없습니다.");
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
        Debug.Log("보호막 깨짐");
        Debug.Log(dmg);
        HP -= dmg;
        
        Debug.Log(dmg + "데미지  현재체력" + HP );
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
