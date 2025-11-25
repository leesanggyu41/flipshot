using UnityEngine;
using Photon.Pun;

public class dedzone : MonoBehaviour
{
    private BattleStone battleStone;
    private goung Goung;
    private battlesang Battlesang;
    private ResultManger resultManger;
    private bool sett = false;


    private void Start()
    {
        resultManger = FindAnyObjectByType<ResultManger>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!resultManger.isendBattle)
        {
            if (sett)
            {
                return;
            }
            if(collision.transform.tag == "wang" && sett == false)
            {
                Goung = collision.transform.GetComponent<goung>();
                Goung.HP = -1;
                sett = true;
            }
            else if(collision.transform.tag == "sang" || collision.transform.tag == "infinity"&& sett == false)
            {
                Battlesang = collision.transform.GetComponent<battlesang>();
                Battlesang.HP = -1;
                sett = true;
            }
            else if(collision.transform.tag != "wang" && collision.transform.tag != "sang" && sett == false)
            {
                battleStone = collision.transform.GetComponent<BattleStone>();
                battleStone.HP = -1;
                sett = true;
            }
        }

    }

}
