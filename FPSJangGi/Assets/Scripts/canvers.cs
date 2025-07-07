using UnityEngine;

public class canvers : MonoBehaviour
{
    public GameObject aaa;
    Animator anim;
    AudioSource openSound;

    void Awake()
    {
        anim = aaa.GetComponent<Animator>();
        openSound = GetComponent<AudioSource>();
    }
    public void dmddkdlt()
    {
        aaa.SetActive(true);
        anim.SetBool("IsOpen", true);
        
    }
    public void XXX()
    {
        aaa.SetActive(false);
        anim.SetBool("IsOpen", false);
    }
}
