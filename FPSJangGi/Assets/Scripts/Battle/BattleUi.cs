using UnityEngine;
using UnityEngine.UI;
using TMPro; // 만약 TextMeshPro를 쓴다면 필요

public class BattleUi : MonoBehaviour
{
    public sklli skillScript; // 스킬 스크립트 참조
    public BattleStone battleStone;
    public goung wang;
    public battlesang sang;
    public Image skillGaugeBar; // 네모 게이지바 (Image, Fill Method: Vertical)
    public Slider hpSlider;     // HP 슬라이더
    public BattleManager gameManager; // 인스펙터에서 할당

    // 외부에서 접근 가능한 값들
    [Header("Status Values")]

    public GameObject aaa;

    public Image passiveGaugeBar; // 패시브 게이지바 (Image, Fill Method: Vertical)
    public bool passiveActive = false;    // 패시브 스킬 활성화 여부

    // 쿨다운 텍스트 UI (TMP_Text)
    // public Text cooldownText; // UnityEngine.UI.Text
    public TMP_Text cooldownText; // TextMeshPro 사용

    public Image skillReadyImage; // 스킬이 다 찼을 때 켜질 이미지 UI

    // 기본공격 쿨타임 UI
    public Image attackCooldownCircle; // 원형 쿨타임 UI (Image, Fill Method: Radial 360)

    public int number;
    

    void Update()
    {
        // 배틀 중이 아니면 UI 비활성화
        if (!gameManager.isgo)
        {
            // SetActive(false) 대신 enabled = false로 마스크 구조 유지
            aaa.SetActive(false);
            // ...필요한 다른 UI 오브젝트도 추가...
            return;
        }
        else
        {
            aaa.SetActive(true);
            //if (cooldownText != null) cooldownText.enabled = false;
            // cooldownText는 아래에서 따로 처리
            // ...필요한 다른 UI 오브젝트도 추가...
        }

        // 액티브 스킬 게이지 (슬라이더/게이지바처럼 값에 따라 바뀜)
        if (skillGaugeBar != null)
        {
            if(skillScript != null)
            {
                float fill = Mathf.Clamp01(skillScript.delay - skillScript.cooldownTimer / Mathf.Max(0.0001f, skillScript.delay));
                // Image 타입이 Filled일 때
                if (skillGaugeBar.type == Image.Type.Filled)
                {
                    skillGaugeBar.fillAmount = fill;
                }
                else
                {
                    // 기존 방식 (sizeDelta.y 조정)
                    RectTransform rt = skillGaugeBar.rectTransform;
                    if (_originalSkillGaugeHeight < 0f)
                        _originalSkillGaugeHeight = rt.sizeDelta.y;
                    rt.pivot = new Vector2(0.5f, 0f);
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, _originalSkillGaugeHeight * fill);
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0f);
                }
            }
            else if(wang != null)
            {
                float fill = Mathf.Clamp01(wang.delay - wang.cooldownTimer / Mathf.Max(0.0001f, wang.delay));
                // Image 타입이 Filled일 때
                if (skillGaugeBar.type == Image.Type.Filled)
                {
                    skillGaugeBar.fillAmount = fill;
                }
                else
                {
                    // 기존 방식 (sizeDelta.y 조정)
                    RectTransform rt = skillGaugeBar.rectTransform;
                    if (_originalSkillGaugeHeight < 0f)
                        _originalSkillGaugeHeight = rt.sizeDelta.y;
                    rt.pivot = new Vector2(0.5f, 0f);
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, _originalSkillGaugeHeight * fill);
                    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0f);
                }
            }


        }

        // 패시브 게이지 (bool 값에 따라 가득/비움)
        if (passiveGaugeBar != null)
        {
            // passiveActive가 true면 이미지 보이게, false면 안 보이게
            passiveGaugeBar.enabled = passiveActive;
        }

        // HP 슬라이더 (public 변수 사용)
        if (hpSlider != null)
        {

            //hpSlider.maxValue = maxHp;
            //hpSlider.value = currentHp;
            if (number == 1)
            {
                hpSlider.value = battleStone.HP;
                hpSlider.maxValue = battleStone.Maxhp;
            }
            else if (number == 2)
            {
                hpSlider.value = wang.HP;
                hpSlider.maxValue = wang.Maxhp;
            }
            else if (number == 3)
                
            {
                hpSlider.value = sang.HP;
                hpSlider.maxValue = sang.Maxhp;
            }
        }

        // 쿨다운 텍스트 UI 처리 (게이지와 별개로)
        if (cooldownText != null)
        {
            if (skillScript != null)
            {
                if (skillScript.cooldownTimer > 0.01f)
                {
                    int remain = Mathf.Max(0, Mathf.CeilToInt(skillScript.cooldownTimer));
                    skillReadyImage.enabled = false;
                    cooldownText.enabled = true;
                    cooldownText.text = $"{remain}";
                }
                else
                {
                    cooldownText.enabled = false;
                    skillReadyImage.enabled = true;
                }
            }
            else if (wang != null)
            {
                if (wang.cooldownTimer > 0.01f)
                {
                    int remain = Mathf.Max(0, Mathf.CeilToInt(wang.cooldownTimer));
                    skillReadyImage.enabled = false;
                    cooldownText.enabled = true;
                    
                    cooldownText.text = $"{remain}";
                }
                else
                {
                    cooldownText.enabled = false;
                    skillReadyImage.enabled = true;
                }
            }


        }

        

        // 기본공격 쿨타임 원형 UI 처리 (마우스 따라다님)
        if (attackCooldownCircle != null)
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                attackCooldownCircle.canvas.transform as RectTransform,
                Input.mousePosition,
                attackCooldownCircle.canvas.worldCamera,
                out mousePos
            );
            attackCooldownCircle.rectTransform.anchoredPosition = mousePos;
            if (number == 1)
            {
                float fill = 1f - Mathf.Clamp01(battleStone.cooldown / Mathf.Max(0.0001f, battleStone.delay));
                attackCooldownCircle.fillAmount = fill;
            }
            else if (number == 2)
            {
                float fill = 1f - Mathf.Clamp01(wang.cooldown / Mathf.Max(0.0001f, wang.delay));
                attackCooldownCircle.fillAmount = fill;
            }
            else if (number == 3)
            {
                float fill = 1f - Mathf.Clamp01(sang.cooldown / Mathf.Max(0.0001f, sang.delay));
                attackCooldownCircle.fillAmount = fill;
            }
            // 쿨타임 진행에 따라 fillAmount 조절 (0~1)

            
        }

        // 기본공격 쿨타임 타이머 증가/초기화는 외부에서 처리 (여기서 증가시키지 않음)
        // attackCooldownTimer += Time.deltaTime;
        // if (attackCooldownTimer > attackCooldown)
        //     attackCooldownTimer = attackCooldown; // 최대값 고정 (삭제)
    }

    float _originalSkillGaugeHeight = -1f;
    float _originalSkillCooldown = -1f;
    float _originalPassiveGaugeHeight = -1f;

    // 배틀 중 여부를 반환하는 메서드 (GameManager에 isBattle이 있다고 가정)
    
}