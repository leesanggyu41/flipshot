using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
// using Cinemachine; // <-- 이 부분에서 오류가 발생한다면 아래 안내를 따르세요.

public class CamManagerScript : MonoBehaviour
{
    [Header("UI 그룹")]
    public GameObject mainUI;
    public GameObject startUI;

    [Header("시네머신 카메라")]
    // CinemachineVirtualCamera 타입에서 오류가 발생한다면 아래처럼 주석 처리하고,
    // Unity 에디터에서 Cinemachine 패키지가 정상적으로 설치되어 있는지 다시 확인하세요.
    // public CinemachineVirtualCamera camMain;
    // public CinemachineVirtualCamera camStart;
    // public CinemachineVirtualCamera camCredit;

    // 임시로 GameObject로 선언하여 에디터 오류를 피할 수 있습니다.
    public GameObject camMain;
    public GameObject camStart;
    public GameObject camCredit;

    [Header("메인 텍스트 효과")]
    public TextMeshProUGUI[] mainFadeTexts;

    [Header("시작 텍스트 효과")]
    public TextMeshProUGUI[] startFadeTexts;

    // UGI Text 관련 필드 제거 (모두 TMP로 통일)

    [Header("카메라 트랜스폼")]
    public Transform mainCamTransform;
    public Transform startCamTransform;
    public Transform creditCamTransform;
    public Camera mainCamera; // 실제 이동시킬 카메라

    [Header("카메라 보간 시간")]
    public float camLerpTime = 1.0f;

    [Header("씬 이름")]
    public string creditSceneName = "CreditScene";

    // 카메라 위치 이동 함수 (즉시 이동)
    public void MoveCameraTo(Transform target)
    {
        if (mainCamera == null || target == null)
            return;
        mainCamera.transform.position = target.position;
        mainCamera.transform.rotation = target.rotation;
    }

    // 카메라 보간 이동 함수
    public void LerpCameraTo(Transform target)
    {
        if (lerpCoroutine != null)
            StopCoroutine(lerpCoroutine);
        lerpCoroutine = StartCoroutine(LerpCameraCoroutine(target));
    }

    private Coroutine lerpCoroutine; // 현재 실행중인 카메라 보간 코루틴

    IEnumerator LerpCameraCoroutine(Transform target)
    {
        if (mainCamera == null || target == null)
            yield break;

        Transform camTr = mainCamera.transform;
        Vector3 startPos = camTr.position;
        Quaternion startRot = camTr.rotation;
        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;

        float t = 0f;
        while (t < camLerpTime)
        {
            float lerpT = Mathf.Clamp01(t / camLerpTime);
            // 부드러운 가속/감속 효과(EaseInOut)
            float smoothT = Mathf.SmoothStep(0f, 1f, lerpT);
            camTr.position = Vector3.Lerp(startPos, endPos, smoothT);
            camTr.rotation = Quaternion.Slerp(startRot, endRot, smoothT);
            t += Time.deltaTime;
            yield return null;
        }
        camTr.position = endPos;
        camTr.rotation = endRot;
    }

    // ▶ 메인 → 시작 UI
    public void OnClickStart()
    {
        LerpCameraTo(startCamTransform);
        StartCoroutine(SwitchUI(mainUI, startUI, mainFadeTexts, startFadeTexts));
    }

    // ▶ 시작 → 메인 UI
    public void OnClickBack()
    {
        LerpCameraTo(mainCamTransform);
        StartCoroutine(SwitchUI(startUI, mainUI, startFadeTexts, mainFadeTexts));
    }

    // ▶ 메인 → 크레딧
    public void OnClickCredit()
    {
        LerpCameraTo(creditCamTransform);
        StartCoroutine(FadeOutAndGoToCredit());
    }

    // 크레딧 진입시 메인 텍스트 페이드 아웃 후 씬 전환
    IEnumerator FadeOutAndGoToCredit()
    {
        // 메인 텍스트 페이드 아웃
        yield return StartCoroutine(FadeTMPText(mainFadeTexts, 1f, 0f, 0.5f));
        // UI 비활성화(선택)
        if (mainUI != null)
            mainUI.SetActive(false);
        // 크레딧 씬 이동
        yield return new WaitForSeconds(1.5f); // 기존 2초와 합산 효과
        SceneManager.LoadScene(creditSceneName);
    }

    IEnumerator SwitchUI(GameObject fromUI, GameObject toUI, TextMeshProUGUI[] fromTexts, TextMeshProUGUI[] toTexts)
    {
        // fromTexts 효과(사라짐)
        yield return StartCoroutine(FadeTMPText(fromTexts, 1f, 0f, 0.5f));

        // 기존 UI 비활성화
        fromUI.SetActive(false);

        // 새 UI 활성화
        toUI.SetActive(true);

        // toTexts 효과(나타남)
        yield return StartCoroutine(FadeTMPText(toTexts, 0f, 1f, 0.5f));
    }

    // TMP 텍스트 보간만 사용
    IEnumerator FadeTMPText(TextMeshProUGUI[] texts, float startWeight, float endWeight, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            float w = Mathf.Lerp(startWeight, endWeight, t / duration);
            foreach (var tmp in texts)
            {
                tmp.fontMaterial.SetFloat("_FaceDilate", w - 1f); // 얇아짐 효과
                Color color = tmp.color;
                color.a = Mathf.Lerp(startWeight, endWeight, t / duration);
                tmp.color = color;
            }
            t += Time.deltaTime;
            yield return null;
        }
        // 마지막 상태 보정
        foreach (var tmp in texts)
        {
            tmp.fontMaterial.SetFloat("_FaceDilate", endWeight - 1f);
            Color color = tmp.color;
            color.a = endWeight;
            tmp.color = color;
        }
    }
}

// Cinemachine 패키지가 설치되어 있어도 IDE에서 오류가 뜨면 위의 방법으로 프로젝트 파일을 재생성하세요.

// Cinemachine 관련 오류는 Unity 프로젝트에 Cinemachine 패키지가 올바르게 설치/연동되지 않았을 때 발생합니다.
// 아래 사항을 반드시 확인하세요:

// 1. Unity 에디터에서 Window > Package Manager로 이동
// 2. "Unity Registry"에서 "Cinemachine" 검색 후 설치
// 3. 설치 후 Unity 에디터를 완전히 종료했다가 다시 실행
// 4. Visual Studio(또는 Rider 등 IDE)도 완전히 종료 후 다시 실행
// 5. Unity에서 "Assets > Open C# Project" 또는 "Edit > Preferences > External Tools > Regenerate project files" 실행
// 6. 그래도 안 되면 프로젝트의 Library 폴더를 삭제 후 Unity 재실행

// 위 과정을 거쳐도 오류가 계속된다면, 패키지 설치가 꼬였거나 IDE가 Unity 패키지 참조를 인식하지 못하는 상황입니다.
// 코드에는 문제가 없으니, 반드시 위 환경 문제를 해결해야 합니다.