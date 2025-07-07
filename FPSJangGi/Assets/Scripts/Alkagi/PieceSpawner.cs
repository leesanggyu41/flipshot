using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.Collections;

public class PieceSpawner : MonoBehaviourPun
{
    public GameObject hanPrefab;  // 한나라 돌 프리팹
    public GameObject choPrefab;  // 초나라 돌 프리팹
    public GameObject hanCam;
    public GameObject choCam;
    public Transform spawnPointHan;  // 한나라 돌 생성 위치
    public Transform spawnPointCho;  // 초나라 돌 생성 위치
    public Transform CamaraPoint;
    public BattleManager battleManager;

    void Start()
    {
        StartCoroutine(WaitAndSpawn());
        
        
            

        


    }
    IEnumerator WaitAndSpawn()
    {
        // 2명이 모두 입장할 때까지 대기
        yield return new WaitUntil(() => PhotonNetwork.CurrentRoom.PlayerCount == 2);

        if (PhotonNetwork.IsMasterClient)
        {
            SpawnAllStonesAndCamera();
        }
        else
        {
            SpawnCameraOnly();
        }
    }

    void SpawnAllStonesAndCamera()
    {
        // Null 체크
        if (hanPrefab == null || choPrefab == null)
        {
            Debug.LogError("One or more prefabs are not assigned!");
            return;
        }

        if (spawnPointHan == null || spawnPointCho == null)
        {
            Debug.LogError("One or more spawn points are not assigned!");
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject HanCam = PhotonNetwork.Instantiate(hanCam.name, CamaraPoint.position, Quaternion.Euler(90, 90, 0));
            // 한나라(마스터) 돌 생성 및 소유권/오너 지정
            GameObject hanStone = PhotonNetwork.Instantiate(hanPrefab.name, spawnPointHan.position, Quaternion.Euler(0, 90, 0));
            AlkagiStone hanScript = hanStone.GetComponentInChildren<AlkagiStone>();
            if (hanScript != null)
            {
                hanScript.photonView.RPC("SetOwner", RpcTarget.AllBuffered, PhotonNetwork.MasterClient.ActorNumber.ToString());
            }

            // 초나라(일반) 돌 생성 및 소유권/오너 지정
            GameObject choStone = PhotonNetwork.Instantiate(choPrefab.name, spawnPointCho.position, Quaternion.Euler(0, -90, 0));
            AlkagiStone choScript = choStone.GetComponentInChildren<AlkagiStone>();
            Player otherPlayer = PhotonNetwork.PlayerListOthers.Length > 0 ? PhotonNetwork.PlayerListOthers[0] : null;

            if (otherPlayer != null && choScript != null)
            {
                choScript.TransferOwnershipTo(otherPlayer);
                choScript.photonView.RPC("SetOwner", RpcTarget.AllBuffered, otherPlayer.ActorNumber.ToString());
            }
            else
            {
                Debug.LogWarning("상대방 플레이어가 없거나 AlkagiStone 스크립트가 없습니다.");
            }
        }
    }
    void SpawnCameraOnly()
    {
        GameObject ChoCam = PhotonNetwork.Instantiate(choCam.name, CamaraPoint.position, Quaternion.Euler(90, 270, 0));
    }
 }