using UnityEngine;  
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class BattleSpawner : MonoBehaviourPun
{
    public string chotag;
    public string hantag;
    public Transform hanpoint; //�ѵ� ���� ���
    public Transform chopoint; //�ʵ� ���� ���
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
            


            Debug.Log("���Ͱ� �Ƴ�! ���̴�!");
            if (PhotonNetwork.IsMasterClient)
            {
                int a = Random.Range(0, pan.Length);
                Debug.Log("�̹��� :" + a);
                PhotonNetwork.Instantiate(pan[a].name, new Vector3(45, 0, 0), Quaternion.Euler(0, 90, 0));

                photonView.RPC("rrr", RpcTarget.All, hantag, chotag);
                Debug.Log(PhotonNetwork.MasterClient.NickName);
                foreach (GameObject prefab in hanstones)
                {
                    if (prefab.tag == hantag) // ���⼭ �̸� ��
                    {
                        GameObject sex = PhotonNetwork.Instantiate(prefab.name, hanpoint.position, Quaternion.Euler(-90, 90, -67.5f));//ã�� ������ ��ĭ!
                        Debug.Log("�ѳ��� ��ȯ");
                        amera[0].han = sex.transform;
                        amera[1].han = sex.transform;


                    }
                }
                Debug.Log("�� ã��");



                foreach (GameObject prefab in chostones)
                {
                    if (prefab.tag == chotag) // ���⼭ �̸� ��
                    {
                        GameObject esx = PhotonNetwork.Instantiate(prefab.name, chopoint.position, Quaternion.Euler(-90, 90, 115)); //ã�� ������ ��ĭ!
                        Debug.Log("�ʳ��� ��ȯ");

                        int viewID = esx.GetComponent<PhotonView>().ViewID;
                        foreach (Ccamera cam in amera)
                        {
                            if (cam.CompareTag("chocam"))
                            {
                                PhotonView camView = cam.GetComponent<PhotonView>();
                                if (camView != null)
                                {
                                    camView.RPC("follow", RpcTarget.Others, viewID);
                                    Debug.Log("chocam �±� ī�޶� follow RPC ȣ����: " + cam.name);
                                }
                            }
                        }
                        return;
                    }
                }
            }
            Debug.Log("�� ã��");
        }
        else if (!turnManager.isMasterTurn && sexton == false)
        {
            
            if (!PhotonNetwork.IsMasterClient)
            {
                int a = Random.Range(0, pan.Length);
                Debug.Log("�̹��� :" + a);
                PhotonNetwork.Instantiate(pan[a].name, new Vector3(45, 0, 0), Quaternion.Euler(0, 90, 0));
                photonView.RPC("rrr", RpcTarget.All, hantag, chotag);
                Debug.Log("�Ϲ� Ŭ���̾�Ʈ");
                foreach (GameObject prefab in chostones)
                {
                    if (prefab.tag == chotag) // ���⼭ �̸� ��
                    {
                        GameObject esx = PhotonNetwork.Instantiate(prefab.name, chopoint.position, Quaternion.Euler(-90, 90, 115)); //ã�� ������ ��ĭ!
                        Debug.Log("�ʳ��� ��ȯ");
                        amera[0].cho = esx.transform;
                        amera[1].cho = esx.transform;
                        
                    }
                }
                Debug.Log("�� ã��");
                foreach (GameObject prefab in hanstones)
                {
                    if (prefab.tag == hantag) // ���⼭ �̸� ��
                    {
                        GameObject sex = PhotonNetwork.Instantiate(prefab.name, hanpoint.position, Quaternion.Euler(-90, 90, -67.5f));//ã�� ������ ��ĭ!
                        Debug.Log("�ѳ��� ��ȯ");

                        int viewID = sex.GetComponent<PhotonView>().ViewID;
                        foreach (Ccamera cam in amera)
                        {
                            if (cam.CompareTag("hancam"))
                            {
                                PhotonView camView = cam.GetComponent<PhotonView>();
                                if (camView != null)
                                {
                                    camView.RPC("follow", RpcTarget.Others, viewID);
                                    Debug.Log("chocam �±� ī�޶� follow RPC ȣ����: " + cam.name);
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
        // Pan ������Ʈ�� ���� ��� ������Ʈ ã��
        pan[] pans = FindObjectsByType<pan>(FindObjectsSortMode.None);

        // 2�� �̻��̸� ù ��°�� ����� �������� ����
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
            hanstone.text = "��";
        }
        else if(han == "ma")
        {
            hanstone.text = "��";
        }
        else if (han == "sang")
        {
            hanstone.text = "��";
        }
        else if (han == "cha")
        {
            hanstone.text = "��";
        }
        else if (han == "po")
        {
            hanstone.text = "��";
        }
        else if (han == "sa")
        {
            hanstone.text = "��";
        }
        else if (han == "wang")
        {
            hanstone.text = "��";
        }

        if (cho == "zzol")
        {
            chostone.text = "��";
        }
        else if (cho == "ma")
        {
            chostone.text = "��";
        }
        else if (cho == "sang")
        {
            chostone.text = "��";
        }
        else if (cho == "cha")
        {
            chostone.text = "��";
        }
        else if (cho == "po")
        {
            chostone.text = "��";
        }
        else if (cho == "sa")
        {
            chostone.text = "��";
        }
        else if (cho == "wang")
        {
            chostone.text = "��";
        }

    }
}
