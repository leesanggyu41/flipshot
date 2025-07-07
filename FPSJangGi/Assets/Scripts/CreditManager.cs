using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditManager : MonoBehaviour
{
    // Reference to the Animator component
    private Animator animator;

    void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check if any key is pressed
        if (Input.anyKeyDown)
        {
            LoadMainScene();
        }

        // Check if the animation has finished
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !animator.IsInTransition(0))
        {
            LoadMainScene();
        }
    }

    private void LoadMainScene()
    {
        // Load the main scene (replace "MainScene" with the actual name of your main scene)
        SceneManager.LoadScene(0);
    }
}
