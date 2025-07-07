using Photon.Pun;
using UnityEngine;

public class DieStone : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    //public Material materials; // �̸� ����ϰų� Resources.Load�� ������ �� ����


    

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = photonView.InstantiationData;
        if (data != null && data.Length > 0)
        {
            string matName = (string)data[0];

            // �ڽĿ� ����
            foreach (Transform t in transform)
            {
                Renderer r = t.GetComponent<Renderer>();
                if (r != null)
                {

                    Material loadedMat = Resources.Load<Material>("Materials/" + matName);
                    if (loadedMat != null) r.material = loadedMat;
                    Debug.Log(loadedMat);

                }
            }
        }
    }

    private void Awake()
    {
        
        Invoke("www", 2f);
    }
    
    void www()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}