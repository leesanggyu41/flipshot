using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 방 하나의 UI 항목을 관리하는 클래스
/// </summary>
public class RoomItem : MonoBehaviour
{
    public TMP_Text roomNameText; // 방 이름 표시 텍스트
    public TMP_Text playerCountText; // 인원 표시 텍스트
    public Button joinButton; // 참가 버튼

    private string roomName; // 방 이름 저장
    private RoomListManager roomListManager; // 방 목록 처리를 위한 매니저 참조

    /// <summary>
    /// UI에 표시할 방 정보 설정
    /// </summary>
    public void Setup(string roomName, int playerCount, int maxPlayers, RoomListManager manager)
    {
        this.roomName = roomName;
        this.roomListManager = manager;

        roomNameText.text = roomName;
        playerCountText.text = $"{playerCount}/{maxPlayers}";

        // 버튼 클릭 시 동작 설정 (중복 제거)
        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() =>
        {
            if (roomListManager != null)
                roomListManager.JoinRoom(roomName);
            else
                Debug.LogError("roomListManager가 null입니다!");
        });
    }
}