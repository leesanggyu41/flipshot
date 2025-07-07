using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using TMPro;


public class ResultManger : MonoBehaviourPun
{

    public TMP_Text result_text;
    private string result;
    public bool Win = false;
    
    public BattleManager battleManager;
    public BattleStone battleStone;
    public goung Goung;
    public battlesang BA;
    private PhotonView battleManagerView;
    public BattleSpawner battleSpawner;
    public TurnManager turnManager;

    private void Update()
    {
        turnManager = FindAnyObjectByType<TurnManager>();
        if(battleManager == null)
        {
            battleManager = FindAnyObjectByType<BattleManager>();
        }
        if (battleSpawner == null)
        {
            battleSpawner = FindAnyObjectByType<BattleSpawner>();
        }
        if (battleManagerView == null)
        {
            battleManagerView = battleManager.GetComponent<PhotonView>();
        }
    }

    [PunRPC]
    public void endbattle(int a)
    {

        Debug.Log(a);
        if (a == 7)
        {
            result = "cho";
            StartCoroutine(waitt());
        }
        else if (a == 8)
        {
            result = "han";
            StartCoroutine(waitt());
        }
        Debug.Log(result);
        
    }



    IEnumerator waitt()
    {
        
        yield return new WaitForSeconds(0.2f);
        battleStone = FindAnyObjectByType<BattleStone>();
        Goung = FindAnyObjectByType<goung>();
        BA = FindAnyObjectByType<battlesang>();

        if (result == "cho")
        {
            if (Goung == null && BA == null)
                battleStone.HP = battleStone.Maxhp;
            else if (battleStone == null && BA == null)
                Goung.HP = Goung.Maxhp;
            else if (Goung == null && battleStone == null)
                BA.HP = BA.Maxhp;
            Debug.Log("초나라 승리");
            Win = false;
            result_text.color = Color.blue;
            result_text.text = "초나라가 승리 했습니다!";
            yield return new WaitForSeconds(2.5f);
            battleSpawner.sexton = false;
            battleManager.isbattle = false;
            result_text.text = "";
            if (Goung == null && BA == null)
                battleStone.delete();
            else if (battleStone == null && BA == null)
                Goung.delete();
            else if (Goung == null && battleStone == null)
                BA.delete();
            
            //battleManagerView.RPC("Alkagimove", RpcTarget.All);

            battleManager.Alkagimove();



            

        }
        else if (result == "han")
        {
            if (Goung == null && BA == null)
                battleStone.HP = battleStone.Maxhp;
            else if (battleStone == null && BA == null)
                Goung.HP = Goung.Maxhp;
            else if (Goung == null && battleStone == null)
                BA.HP = BA.Maxhp;
            Debug.Log("한나라 승리");
            Win = true;
            result_text.color = Color.red;
            result_text.text = "한나라가 승리 했습니다!";
            yield return new WaitForSeconds(2.5f);
            battleSpawner.sexton = false;
            battleManager.isbattle = false;
            result_text.text = "";
            if (Goung == null && BA == null)
                battleStone.delete();
            else if (battleStone == null && BA == null)
            Goung.delete();
            else if (Goung == null && battleStone == null)
                BA.delete();


            //battleManagerView.RPC("Alkagimove", RpcTarget.All);
            battleManager.Alkagimove();

            

        }

        
    }

    
}
