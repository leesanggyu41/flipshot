using UnityEngine;

public class setting : MonoBehaviour
{
    public GameObject SettingPanel;
    public Animator anim;
    AudioSource openSound;

    void Awake()
    {
        anim = GetComponent<Animator>();
        openSound = GetComponent<AudioSource>();
    }

    public void CloseSettingPanel()
    {
        SettingPanel.SetActive(false);
        anim.SetBool("IsOpen", false);
    }
    public void OpenSettingPanel()
    {
        anim.SetBool("IsOpen", true);
        if (openSound != null)
            openSound.Play();
    }
}
