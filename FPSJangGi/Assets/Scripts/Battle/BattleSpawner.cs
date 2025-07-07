using UnityEngine;  
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class BattleSpawner : MonoBehaviourPun
{
    public string chotag;
    public string hantag;
    public Transform hanpoint; //한돌 스폰 장소
    public Transform chopoint; //초돌 스폰 장소
    public TurnManager turnManager;
    [SerializeField]
    private GameObject[] hanstones;
    [SerializeField]
    private GameObject[] chostones;
    public bool sexton = false;
    public Ccamera[] amera;

    private PhotonView CameraView;

    public GameObject[] pan;

    string guestName;

    public TMP_Text hanname;
    public TMP_Text hanstone;
    public TMP_Text choname;
    public TMP_Text chostone;
    


    public void Start()
    {
        turnManager = FindAnyObjectByType<TurnManager>();

        hanname.text = PhotonNetwork.MasterClient.NickName;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.IsMasterClient)
            {
                guestName = player.NickName;
                break;
            }
        }
        choname.text =  guestName;

    }

    public void Update()
    {
        if (amera.Length < 2)
        {
            amera = FindObjectsByType<Ccamera>(FindObjectsSortMode.None);
        }
    }

    public void stoneSpawn()
    {
        
;

        if (turnManager.isMasterTurn && sexton == false)
        {
            


            Debug.Log("몬스터가 아냐! 신이다!");
            if (PhotonNetwork.IsMasterClient)
            {
                int a = Random.Range(0, pan.Length);
                Debug.Log("이번맵 :" + a);
                PhotonNetwork.Instantiate(pan[a].name, new Vector3(45, 0, 0), Quaternion.Euler(0, 90, 0));

                photonView.RPC("rrr", RpcTarget.All, hantag, chotag);
                Debug.Log(PhotonNetwork.MasterClient.NickName);
                foreach (GameObject prefab in hanstones)
                {
                    if (prefab.tag == hantag) // 여기서 이름 비교
                    {
                        GameObject sex = PhotonNetwork.Instantiate(prefab.name, hanpoint.position, Quaternion.Euler(-90, 90, -67.5f));//찾은 프리팹 쇼칸!
                        Debug.Log("한나라 소환");
                        amera[0].han = sex.transform;
                        amera[1].han = sex.transform;


                    }
                }
                Debug.Log("못 찾음");



                foreach (GameObject prefab in chostones)
                {
                    if (prefab.tag == chotag) // 여기서 이름 비교
                    {
                        GameObject esx = PhotonNetwork.Instantiate(prefab.name, chopoint.position, Quaternion.Euler(-90, 90, 115)); //찾은 프리팹 쇼칸!
                        Debug.Log("초나라 소환");

                        int viewID = esx.GetComponent<PhotonView>().ViewID;
                        foreach (Ccamera cam in amera)
                        {
                            if (cam.CompareTag("chocam"))
                            {
                                PhotonView camView = cam.GetComponent<PhotonView>();
                                if (camView != null)
                                {
                                    camView.RPC("follow", RpcTarget.Others, viewID);
                                    Debug.Log("chocam 태그 카메라에 follow RPC 호출함: " + cam.name);
                                }
                            }
                        }
                        return;
                    }
                }
            }
            Debug.Log("못 찾음");
        }
        else if (!turnManager.isMasterTurn && sexton == false)
        {
            
            if (!PhotonNetwork.IsMasterClient)
            {
                int a = Random.Range(0, pan.Length);
                Debug.Log("이번맵 :" + a);
                PhotonNetwork.Instantiate(pan[a].name, new Vector3(45, 0, 0), Quaternion.Euler(0, 90, 0));
                photonView.RPC("rrr", RpcTarget.All, hantag, chotag);
                Debug.Log("일반 클라이언트");
                foreach (GameObject prefab in chostones)
                {
                    if (prefab.tag == chotag) // 여기서 이름 비교
                    {
                        GameObject esx = PhotonNetwork.Instantiate(prefab.name, chopoint.position, Quaternion.Euler(-90, 90, 115)); //찾은 프리팹 쇼칸!
                        Debug.Log("초나라 소환");
                        amera[0].cho = esx.transform;
                        amera[1].cho = esx.transform;
                        
                    }
                }
                Debug.Log("못 찾음");
                foreach (GameObject prefab in hanstones)
                {
                    if (prefab.tag == hantag) // 여기서 이름 비교
                    {
                        GameObject sex = PhotonNetwork.Instantiate(prefab.name, hanpoint.position, Quaternion.Euler(-90, 90, -67.5f));//찾은 프리팹 쇼칸!
                        Debug.Log("한나라 소환");

                        int viewID = sex.GetComponent<PhotonView>().ViewID;
                        foreach (Ccamera cam in amera)
                        {
                            if (cam.CompareTag("hancam"))
                            {
                                PhotonView camView = cam.GetComponent<PhotonView>();
                                if (camView != null)
                                {
                                    camView.RPC("follow", RpcTarget.Others, viewID);
                                    Debug.Log("chocam 태그 카메라에 follow RPC 호출함: " + cam.name);
                                }
                            }
                        }

                    }
                    
                }
                return;
                


                
            }
        }










        photonView.RPC("mapdes", RpcTarget.All);

    }

    [PunRPC]
    void mapdes()
    {
        // Pan 컴포넌트를 가진 모든 오브젝트 찾기
        pan[] pans = FindObjectsByType<pan>(FindObjectsSortMode.None);

        // 2개 이상이면 첫 번째만 남기고 나머지는 삭제
        if (pans.Length > 1)
        {
            for (int i = 1; i < pans.Length; i++)
            {
                PhotonNetwork.Destroy(pans[i].gameObject);
            }
        }
    }

    [PunRPC]
    void rrr(string han, string cho)
    {
        if(han == "zzol")
        {
            hanstone.text = "병";
        }
        else if(han == "ma")
        {
            hanstone.text = "마";
        }
        else if (han == "sang")
        {
            hanstone.text = "상";
        }
        else if (han == "cha")
        {
            hanstone.text = "차";
        }
        else if (han == "po")
        {
            hanstone.text = "포";
        }
        else if (han == "sa")
        {
            hanstone.text = "사";
        }
        else if (han == "wang")
        {
            hanstone.text = "궁";
        }

        if (cho == "zzol")
        {
            chostone.text = "병";
        }
        else if (cho == "ma")
        {
            chostone.text = "마";
        }
        else if (cho == "sang")
        {
            chostone.text = "상";
        }
        else if (cho == "cha")
        {
            chostone.text = "차";
        }
        else if (cho == "po")
        {
            chostone.text = "포";
        }
        else if (cho == "sa")
        {
            chostone.text = "사";
        }
        else if (cho == "wang")
        {
            chostone.text = "궁";
        }

    }
}
