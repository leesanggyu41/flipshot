using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    // 크레딧 씬으로 이동
    public void OnCreditButton()
    {
        SceneManager.LoadScene("Credit"); // 크레딧 씬 이름 정확히 입력
    }

    // 설정 패널 토글
    public void OnSettingsButton()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }
    public void OffSettingsButton()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
    // 종료
    public void OnExitButton()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 실행 시 종료
#endif
    }
}
