using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class destroyzone : MonoBehaviourPun
{
    public BattleManager battleManager;
    public PhotonView battleManagerView;

    public TurnManager turnManager;
    public PhotonView turnView;


    private void Start()
    {
        battleManager = FindAnyObjectByType<BattleManager>();
        battleManagerView = battleManager.GetComponent<PhotonView>();

        turnManager = FindAnyObjectByType<TurnManager>();
        turnView = turnView.GetComponent<PhotonView>();
    }

    public void OnTriggerEnter(Collider collision)
    {
        AlkagiStone aaa = collision.GetComponent<AlkagiStone>();
        if (aaa.isShot)
        {
            if (collision.gameObject.layer == 7)
            {
                Debug.Log("한");
                battleManager.handie();
                //battleManagerView.RPC("handie", RpcTarget.All);
            }
            else if (collision.gameObject.layer == 8)
            {
                Debug.Log("초");
                battleManager.chodie();
                //battleManagerView.RPC("chodie", RpcTarget.All);
            }
            if(turnView == null) turnView = turnView.GetComponent<PhotonView>();
            turnView.RPC("EndTurnRPC", RpcTarget.All); 
            Debug.Log(collision);
            Destroy(collision);
        }
        else
        {
            if (collision.gameObject.layer == 7)
            {
                Debug.Log("한");
                battleManager.handie();
                //battleManagerView.RPC("handie", RpcTarget.All);
            }
            else if (collision.gameObject.layer == 8)
            {
                Debug.Log("초");
                battleManager.chodie();
                //battleManagerView.RPC("chodie", RpcTarget.All);
            }

            
            Debug.Log(collision);
            Destroy(collision);
        }

            
        
    }

}
