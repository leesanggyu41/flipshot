using UnityEngine;
using Photon.Pun;

public class www : MonoBehaviourPun
{
    public Camera cam;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            cam = GameObject.FindGameObjectWithTag("chocam").GetComponent<Camera>();
        }
    }
    void Update()
    {
        if (Camera.main == null) return;

        

        if(!PhotonNetwork.IsMasterClient) transform.Rotate(90f,0f,180f);
        // �Ųٷ� ������ �ʵ��� 180�� ȸ��

    }
}