using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class NicknameManager : MonoBehaviour
{
    public TMP_InputField nicknameInput; // 닉네임 입력 필드
    public Button confirmNicknameButton; // 닉네임 확인 버튼

    private void Start()
    {
        // 닉네임 버튼 비활성화 (닉네임이 입력될 때까지)
        confirmNicknameButton.interactable = false;

        // InputField 이벤트 설정
        nicknameInput.onValueChanged.AddListener(OnNicknameChanged);
        nicknameInput.onEndEdit.AddListener(OnNicknameEnter);
    }

    // 닉네임 입력 필드 값이 변경될 때 호출
    private void OnNicknameChanged(string text)
    {
        confirmNicknameButton.interactable = !string.IsNullOrEmpty(text);
    }

    // 엔터 키 입력 시 닉네임 자동 저장
    private void OnNicknameEnter(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SetNickname();
        }
    }

    // 닉네임 설정 버튼 클릭 시 실행
    public void SetNickname()
    {
        Debug.Log("응");
        if (!string.IsNullOrEmpty(nicknameInput.text))
        {
            PhotonNetwork.NickName = nicknameInput.text; // 포톤 네트워크에 닉네임 설정
            Debug.Log("닉네임 설정됨: " + PhotonNetwork.NickName);
        }
    }
}