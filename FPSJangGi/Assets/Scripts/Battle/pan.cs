using UnityEngine;
using Photon.Pun;
public class pan : MonoBehaviourPun
{
    public BattleManager battleManager;

    private void Awake()
    {
        battleManager = FindAnyObjectByType<BattleManager>();
    }

    private void Start()
    {
        if(battleManager == null) battleManager = FindAnyObjectByType<BattleManager>();
        battleManager.Pan = this;
    }
    [PunRPC]
    public void des()
    {
        Destroy(gameObject);
    }
}
