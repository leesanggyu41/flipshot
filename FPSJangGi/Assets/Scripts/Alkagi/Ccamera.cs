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
            Debug.Log("카메라 이전");
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
        

        Debug.Log("받은 viewID: " + viewID);


        PhotonView view = PhotonView.Find(viewID);

        if (view == null)
        {
            Debug.LogWarning("해당 viewID의 PhotonView를 찾을 수 없습니다.");
            return;
        }

        Debug.Log("찾은 PhotonView: " + view.name);
        
        if (!PhotonNetwork.IsMasterClient)
        {
            cho = view.gameObject.transform;
            Debug.Log("Master - cho 저장 완료: " + cho.name);
        }
        else
        {
            han = view.gameObject.transform;
            Debug.Log("Client - han 저장 완료: " + han.name);
        }
        
        


        if (cho == null && han == null)
        {
            Debug.Log("Transform이 저장되지 않음. 다시 시도 중...");
            StartCoroutine(RetryFollow(viewID));
        }
    }
   

    IEnumerator RetryFollow(int viewID)
    {
        yield return new WaitForSeconds(0.5f); // 0.5초 후 재시도
        follow(viewID);
    }




} 
