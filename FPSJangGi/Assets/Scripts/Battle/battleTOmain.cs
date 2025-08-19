using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class battleTOmain : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private GameObject qiut; //정말 나가기 UI
    [SerializeField]
    private GameObject setting;
    private bool arr = false;

    [SerializeField]
    private AudioMixer master;

    public Slider ma;
    public Slider bgm;
    public Slider eff;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!arr)
            {
                setting.SetActive(true);
                float dB;
                master.GetFloat("Master", out dB);
                float value = Mathf.Pow(10f, dB / 20f);
                bgm.value = value;
                float dB1;
                master.GetFloat("BGM", out dB1);
                float value1 = Mathf.Pow(10f, dB1 / 20f);
                bgm.value = value1;
                float dB2;
                master.GetFloat("Effect", out dB2);
                float value2 = Mathf.Pow(10f, dB2 / 20f);
                bgm.value = value2;
                arr = !arr;
            }
            else
            {
                setting.SetActive(false);
                arr = !arr;
            }
        }
    }

    public void plquit()
    {
        qiut.SetActive(true);
    }

    public void cancle()
    {
        qiut.SetActive(false);
    }

    public void yes()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
