using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class TurnManager : MonoBehaviourPun
{
    public static TurnManager Instance;
    public bool isMasterTurn = true;
    public BattleManager battleManager;
    public bool turing = false;

    public GameObject han;

    public GameObject cho;
    void Awake()
    {
        Instance = this;
        battleManager = FindAnyObjectByType<BattleManager>();
    }

    // 모든 클라이언트에 턴 종료 + 소유권 이전 동기화
    public void RequestEndTurn()
    {

        photonView.RPC(nameof(EndTurnRPC), RpcTarget.All);
    }

    [PunRPC]
    public void EndTurnRPC()
    {

        if (turing) return;
        turing = true;
        if (!isMasterTurn)
        {
            han.SetActive(true);
            StartCoroutine(krto());
        }
        else
        {
            cho.SetActive(true);
            StartCoroutine(krto());
        }
        battleManager.isup = false;
        // 1) 먼저 턴 상태를 뒤집고

        isMasterTurn = !isMasterTurn;
        PhotonView battleView = battleManager.GetComponent<PhotonView>();

        battleView.RPC("endBattle", RpcTarget.All);
        // 2) 소유권 이전은 MasterClient 한 번만 실행
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("ChangeStonesOwnership", RpcTarget.All);

        // 3) UI/로그 갱신
        Debug.Log(isMasterTurn
            ? "현재 턴: 마스터클라이언트"
            : "현재 턴: 일반클라이언트");


    }


    IEnumerator krto()
    {
        yield return new WaitForSeconds(3);
        turing = false;
        battleManager.usshot = false;
        han.SetActive(false);
        cho.SetActive(false);

    }
        [PunRPC]
    void ChangeStonesOwnership()
    {
        Player next = isMasterTurn
            ? PhotonNetwork.MasterClient   // 뒤집힌 후 isMasterTurn=true → MasterClient 차례
            :  GetOtherPlayer();            // isMasterTurn=false → OtherPlayer 차례

        foreach (var stone in FindObjectsOfType<AlkagiStone>())
        {
            stone.TransferOwnershipTo(next);
        }
    }
    Player GetOtherPlayer()
    {
        foreach (var p in PhotonNetwork.PlayerList)
            if (!p.IsMasterClient) return p;
        return null;
    }



    public bool CanTakeTurn()
    {
        // 내 턴일 때만 true
        if (isMasterTurn)
            return PhotonNetwork.IsMasterClient;
        else
            return !PhotonNetwork.IsMasterClient;
    }
}
