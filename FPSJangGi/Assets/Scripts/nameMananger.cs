using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class NicknameManager : MonoBehaviour
{
    public TMP_InputField nicknameInput; // �г��� �Է� �ʵ�
    public Button confirmNicknameButton; // �г��� Ȯ�� ��ư

    private void Start()
    {
        // �г��� ��ư ��Ȱ��ȭ (�г����� �Էµ� ������)
        confirmNicknameButton.interactable = false;

        // InputField �̺�Ʈ ����
        nicknameInput.onValueChanged.AddListener(OnNicknameChanged);
        nicknameInput.onEndEdit.AddListener(OnNicknameEnter);
    }

    // �г��� �Է� �ʵ� ���� ����� �� ȣ��
    private void OnNicknameChanged(string text)
    {
        confirmNicknameButton.interactable = !string.IsNullOrEmpty(text);
    }

    // ���� Ű �Է� �� �г��� �ڵ� ����
    private void OnNicknameEnter(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SetNickname();
        }
    }

    // �г��� ���� ��ư Ŭ�� �� ����
    public void SetNickname()
    {
        Debug.Log("��");
        if (!string.IsNullOrEmpty(nicknameInput.text))
        {
            PhotonNetwork.NickName = nicknameInput.text; // ���� ��Ʈ��ũ�� �г��� ����
            Debug.Log("�г��� ������: " + PhotonNetwork.NickName);
        }
    }
}