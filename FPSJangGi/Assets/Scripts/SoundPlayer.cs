using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource audioSource;

    // �ִϸ��̼� �̺�Ʈ���� ȣ���� �Լ�
    public void PlaySound()
    {
        
            audioSource.Play();
        
    }
}