using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class Ccamera : MonoBehaviourPun
{


    public BattleManager battleManager;
    public BattleSpawner battleSpawner;

    public Transform han;
    public Transform cho;

    private Vector3 offset = new Vector3(0f, 4f, 0f);

    public float followSpeed = 100f;

    public TurnManager turnManager;

    [PunRPC]
    void Start()
    {
        

        battleManager = FindAnyObjectByType<BattleManager>();
        battleSpawner = FindAnyObjectByType<BattleSpawner>();
        turnManager = FindAnyObjectByType<TurnManager>();

        transform.SetParent(null);


    }





    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient && !this.photonView.IsMine && gameObject.layer == 8)
        {

            this.photonView.RequestOwnership();
            Debug.Log("ī�޶� ����");
        }
        if (battleManager.chocam != null && battleManager.hancam != null)
        {
            if (photonView.IsMine)
            {
                photonView.gameObject.SetActive(true);
            }
            else
            {
                photonView.gameObject.SetActive(false);
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if (han != null)
            {
                Vector3 desiredPosition = han.position + offset;
                transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (cho != null)
            {
                Vector3 desiredPosition = cho.position + offset;
                transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            }

        }


    }

    [PunRPC]
    public void follow(int viewID)
    {
        

        Debug.Log("���� viewID: " + viewID);


        PhotonView view = PhotonView.Find(viewID);

        if (view == null)
        {
            Debug.LogWarning("�ش� viewID�� PhotonView�� ã�� �� �����ϴ�.");
            return;
        }

        Debug.Log("ã�� PhotonView: " + view.name);
        
        if (!PhotonNetwork.IsMasterClient)
        {
            cho = view.gameObject.transform;
            Debug.Log("Master - cho ���� �Ϸ�: " + cho.name);
        }
        else
        {
            han = view.gameObject.transform;
            Debug.Log("Client - han ���� �Ϸ�: " + han.name);
        }
        
        


        if (cho == null && han == null)
        {
            Debug.Log("Transform�� ������� ����. �ٽ� �õ� ��...");
            StartCoroutine(RetryFollow(viewID));
        }
    }
   

    IEnumerator RetryFollow(int viewID)
    {
        yield return new WaitForSeconds(0.5f); // 0.5�� �� ��õ�
        follow(viewID);
    }




} 
