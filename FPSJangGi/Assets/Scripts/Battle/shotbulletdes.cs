using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class shotbulletdes : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = photonView.InstantiationData;

        if (data != null && data.Length > 0)
        {
            int shooterID = (int)data[0];
            PhotonView shooterPV = PhotonView.Find(shooterID);

            if (shooterPV != null)
            {
                Transform shooterTransform = shooterPV.transform;

                // 자식 총알들에게 발사자 전달
                Bullet[] bullets = GetComponentsInChildren<Bullet>();
                foreach (Bullet bullet in bullets)
                {
                    bullet.SetShooter(shooterTransform);
                }
            }
        }
    }

    void Start()
    {
        Invoke("des", 5f);
    }

    public void des()
    {
        PhotonNetwork.Destroy(gameObject);
    }
    
}
