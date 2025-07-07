using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource audioSource;

    // 애니메이션 이벤트에서 호출할 함수
    public void PlaySound()
    {
        
            audioSource.Play();
        
    }
}