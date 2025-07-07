using UnityEngine;
using Photon.Pun;

public class particledestroy : MonoBehaviourPun
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("ww", 2f);
    }

    void ww()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
