using UnityEngine;
using Photon.Pun;

public class dedzone : MonoBehaviour
{
    private BattleStone battleStone;
    private goung Goung;
    private battlesang Battlesang;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "wang")
        {
            Goung = collision.transform.GetComponent<goung>();
            Goung.HP = -1;
        }
        else if(collision.transform.tag == "sang")
        {
            Battlesang = collision.transform.GetComponent<battlesang>();
            Battlesang.HP = -1; 
        }
        else
        {
            battleStone = collision.transform.GetComponent<BattleStone>();
            battleStone.HP = -1;
        }
    }

}
