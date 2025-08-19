using UnityEngine;
using Photon.Pun;

public class RamdomRoomJoin : MonoBehaviourPunCallbacks
{
    RoomListManager roomlistmg;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RandomRoomJoin()
    {
        PhotonNetwork.JoinRandomRoom(); // 랜덤 방에 입장
        Debug.Log("랜덤 방에 입장 시도 중...");
    }
}
