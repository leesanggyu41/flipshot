using UnityEngine;
using Photon.Pun;
using System.Collections;

public class LoadSceneManager : MonoBehaviourPunCallbacks
{
    public static LoadSceneManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadFPSScene(Vector3 collisionForce, string sourceName, string targetName)
    {
        if (!photonView)
        {
            Debug.LogError("PhotonView가 없습니다!");
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("충돌 감지 - 씬 전환 준비");
            
            // 충돌 데이터 저장
            CollisionDataCache.cachedForce = collisionForce;
            CollisionDataCache.sourceStoneName = sourceName;
            CollisionDataCache.targetStoneName = targetName;

            // 모든 클라이언트에게 씬 전환 준비를 알림
            photonView.RPC("PrepareForSceneChange", RpcTarget.All);
        }
    }

    [PunRPC]
    private void PrepareForSceneChange()
    {
        Debug.Log($"PrepareForSceneChange 실행 - 클라이언트: {(PhotonNetwork.IsMasterClient ? "마스터" : "게스트")}");
        StartCoroutine(LoadFPSSceneCoroutine());
    }

    private IEnumerator LoadFPSSceneCoroutine()
    {
        Debug.Log("FPS 씬 로딩 시작");
        
        // 씬 동기화 설정
        PhotonNetwork.AutomaticallySyncScene = true;
        
        // 씬 전환 전 약간의 지연
        yield return new WaitForSecondsRealtime(0.1f);
        
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("마스터 클라이언트가 FPS 씬으로 전환 시도");
            PhotonNetwork.LoadLevel("FPSscene");
        }
        else
        {
            Debug.Log("게스트 클라이언트가 마스터의 씬 전환 대기 중");
        }
    }

    // 충돌 데이터 저장을 위한 정적 클래스
    public static class CollisionDataCache
    {
        public static Vector3 cachedForce = Vector3.zero;
        public static string sourceStoneName = "";
        public static string targetStoneName = "";
    }
}