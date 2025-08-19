using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.Rendering;

[System.Serializable]
public class SettingDate
{
    public float BGM = 1f;
    public float SFX = 1f;
    public float MasterSound = 1f;

    public KeyCode AbilityKey = KeyCode.Mouse1;
    public List<KeyCode> MoveKeys = new List<KeyCode>
    {
        KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D
    };

    public int QulityLevel = 2;
    public int DisplayResolution = 16;
    public bool isFullScreen = true;
}

public class SettingSave : MonoBehaviour
{

    
    
    public static SettingSave instance;
    [Header("AudioMixer")]
    public AudioMixer audioMixer;
    [Header("Reset")]
    public Button ResetButton;

    [Header("Tab")]
    public List<Button> TabButton = new List<Button>();
    public List<GameObject> UIPanel = new List<GameObject>();

    [Header("Sound")]
    public Slider BGM;
    public Slider SFX, MasterSound;
    public TMP_Text BGMText, SFXText, MasterSoundText;

    [Header("KeyMap")]
    public TMP_Text currentKeyText;
    public GameObject KeySetPanel;
    public List<Button> MoveKeyButtons = new List<Button>();
    public List<TMP_Text> MoveKeyTexts = new List<TMP_Text>();

    [Header("Qulity")]
    public TMP_Dropdown QulityDropDown;

    [Header("Resolution")]
    public TMP_Text ResolutionLabel;
    public Button RigthButton, LeftButton;
    private List<Resolution> filteredResolution = new List<Resolution>();
    private int CutrrentResolution;

    [Header("Full Screen")]
    public Toggle FullcrrenToggle;

    private string path;
    private SettingDate settingDate = new SettingDate();

    private bool isKeySetting = false;
    private int moveKeyTargetIndex = -1;


    

    void Awake()
    {
        path = Path.Combine(Application.persistentDataPath, "SettingDate.json");
        LoadDate();

        ResetButton.onClick.AddListener(ResetToDefault);

        QulityDropDown.ClearOptions();
        QulityDropDown.AddOptions(new List<string>(QualitySettings.names));
        QulityDropDown.value = settingDate.QulityLevel;
        QulityDropDown.onValueChanged.AddListener(OnQulityChange);
        QualitySettings.SetQualityLevel(settingDate.QulityLevel, true);

        InitResolutionList();
        ApplyResolution(settingDate.DisplayResolution);
        RigthButton.onClick.AddListener(() => ChangeResolution(1));
        LeftButton.onClick.AddListener(() => ChangeResolution(-1));

        FullcrrenToggle.onValueChanged.AddListener(OnFullScreenToggleChange);
        Screen.fullScreen = settingDate.isFullScreen;

        BGM.onValueChanged.AddListener((value) =>
        {
            settingDate.BGM = value;
            SetBGMVolume(value);
            SaveDate();
        });
        SetBGMVolume(settingDate.BGM);
        BGM.value = settingDate.BGM;

        SFX.onValueChanged.AddListener((value) =>
        {
            settingDate.SFX = value;
            SetSFXVolume(value);
            SaveDate();
        });
        SetSFXVolume(settingDate.SFX);
        SFX.value = settingDate.SFX;

        MasterSound.onValueChanged.AddListener((value) =>
        {
            settingDate.MasterSound = value;
            SetMasterVolume(value);
            SaveDate();
        });
        SetMasterVolume(settingDate.MasterSound);
        MasterSound.value = settingDate.MasterSound;


        for (int i = 0; i < MoveKeyButtons.Count; i++)
        {
            int index = i;
            MoveKeyButtons[i].onClick.AddListener(() => StartMoveKeyBinding(index));
        }

        UpdateMoveKeyTexts();
    }
    
    void Update()
    {
        BGMText.text = $"{(BGM.value * 100):F0}";
        SFXText.text = $"{(SFX.value * 100):F0}";
        MasterSoundText.text = $"{(MasterSound.value * 100):F0}";

        currentKeyText.text = GetLocalizedKeyName(settingDate.AbilityKey);
        for (int i = 0; i < MoveKeyTexts.Count; i++)
        {
            MoveKeyTexts[i].text = GetLocalizedKeyName(settingDate.MoveKeys[i]);
        }

        if (isKeySetting)
        {
            
           
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isKeySetting = false;
                moveKeyTargetIndex = -1;
                KeySetPanel.SetActive(false);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0)) return;

            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    if (moveKeyTargetIndex >= 0)
                    {
                        settingDate.MoveKeys[moveKeyTargetIndex] = key;
                        UpdateMoveKeyTexts();
                    }
                    else
                    {
                        settingDate.AbilityKey = key;
                        currentKeyText.text = key.ToString();
                    }
                    SaveDate();
                    isKeySetting = false;
                    moveKeyTargetIndex = -1;
                    KeySetPanel.SetActive(false);
                    break;
                }
            }
        }
    }
    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }

    public void SetBGMVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        Debug.Log($"[BGM] Value: {value}, dB: {dB}");
        audioMixer.SetFloat("BGM", dB);

    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("Effect", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }
    void StartMoveKeyBinding(int index)
    {
        moveKeyTargetIndex = index;
        isKeySetting = true;
        KeySetPanel.SetActive(true);
    }

    void UpdateMoveKeyTexts()
    {
        for (int i = 0; i < MoveKeyTexts.Count; i++)
        {
            MoveKeyTexts[i].text = settingDate.MoveKeys[i].ToString();
        }
    }

    void SaveDate()
    {
        string JsonDate = JsonUtility.ToJson(settingDate);
        File.WriteAllText(path, JsonDate);
    }

    void LoadDate()
    {
        if (File.Exists(path))
        {
            string JsonDate = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(JsonDate, settingDate);
        }
        else
        {
            string JsonDate = JsonUtility.ToJson(settingDate);
            File.WriteAllText(path, JsonDate);
        }
    }

    public void ResetToDefault()
    {
        settingDate = new SettingDate();
        BGM.value = settingDate.BGM;
        SFX.value = settingDate.SFX;
        MasterSound.value = settingDate.MasterSound;

        QulityDropDown.value = settingDate.QulityLevel;
        QualitySettings.SetQualityLevel(settingDate.QulityLevel, true);

        CutrrentResolution = Mathf.Clamp(settingDate.DisplayResolution, 0, filteredResolution.Count - 1);
        ApplyResolution(CutrrentResolution);

        FullcrrenToggle.isOn = settingDate.isFullScreen;
        Screen.fullScreen = settingDate.isFullScreen;

        UpdateMoveKeyTexts();
        currentKeyText.text = settingDate.AbilityKey.ToString();
        SaveDate();
    }

    void OnQulityChange(int Index)
    {
        QualitySettings.SetQualityLevel(Index, true);
        settingDate.QulityLevel = Index;
        SaveDate();
    }

    void InitResolutionList()
    {
        HashSet<string> seen = new();
        foreach (var res in Screen.resolutions)
        {
            string key = res.width + "x" + res.height;
            if (!seen.Contains(key))
            {
                filteredResolution.Add(res);
                seen.Add(key);
            }
        }
        CutrrentResolution = Mathf.Clamp(settingDate.DisplayResolution, 0, filteredResolution.Count - 1);
    }

    void ChangeResolution(int direction)
    {
        CutrrentResolution += direction;
        CutrrentResolution = Mathf.Clamp(CutrrentResolution, 0, filteredResolution.Count - 1);
        settingDate.DisplayResolution = CutrrentResolution;
        ApplyResolution(CutrrentResolution);
        SaveDate();
    }

    void ApplyResolution(int Index)
    {
        Resolution res = filteredResolution[Index];
        Screen.SetResolution(res.width, res.height, settingDate.isFullScreen);
        ResolutionLabel.text = $"{res.width} x {res.height}";
    }

    public void OnFullScreenToggleChange(bool isFullScreen)
    {
        settingDate.isFullScreen = isFullScreen;
        SaveDate();
        StartCoroutine(ApplyFullScreenChange());
    }

    private IEnumerator ApplyFullScreenChange()
    {
        yield return new WaitForEndOfFrame();
        Resolution res = filteredResolution[CutrrentResolution];
        Screen.SetResolution(res.width, res.height, settingDate.isFullScreen);
    }

    public void OnSoundTab()
    {
        UIPanel[0].SetActive(true);
        UIPanel[1].SetActive(false);
        UIPanel[2].SetActive(false);
    }

    public void OnGrapicTab()
    {
        Debug.Log("sex");
        UIPanel[0].SetActive(false);
        UIPanel[1].SetActive(true);
        UIPanel[2].SetActive(false);
    }

    public void OnKeyMapTab()
    {
        UIPanel[0].SetActive(false);
        UIPanel[1].SetActive(false);
        UIPanel[2].SetActive(true);
    }

    public void KeySetting()
    {
        moveKeyTargetIndex = -1;
        KeySetPanel.SetActive(true);
        isKeySetting = true;
    }

    string GetLocalizedKeyName(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Mouse0: return "왼쪽 클릭";
            case KeyCode.Mouse1: return "오른쪽 클릭";
            case KeyCode.Mouse2: return "가운데 클릭";
            case KeyCode.LeftShift: return "왼쪽 쉬프트";
            case KeyCode.RightShift: return "오른쪽 쉬프트";
            case KeyCode.UpArrow: return "위쪽 화살표";
            case KeyCode.DownArrow: return "아래쪽 화살표";
            case KeyCode.RightArrow: return "오른쪽 화살표";
            case KeyCode.LeftArrow: return "왼쪽 화살표";
            case KeyCode.LeftControl: return "Ctrl";
            case KeyCode.RightControl: return "한자";
            case KeyCode.LeftAlt: return "Alt";
            case KeyCode.RightAlt: return "한/영";
            case KeyCode.LeftBracket: return "[";
            case KeyCode.RightBracket: return "]";
            case KeyCode.Alpha0: return "0";
            case KeyCode.Alpha1: return "1";
            case KeyCode.Alpha2: return "2";
            case KeyCode.Alpha3: return "3";
            case KeyCode.Alpha4: return "4";
            case KeyCode.Alpha5: return "5";
            case KeyCode.Alpha6: return "6";
            case KeyCode.Alpha7: return "7";
            case KeyCode.Alpha8: return "8";
            case KeyCode.Alpha9: return "9";
            case KeyCode.Plus: return "+";
            case KeyCode.Minus: return "-";
            case KeyCode.Equals: return "=";
            case KeyCode.Underscore: return "_";
            case KeyCode.Slash: return "/";
            case KeyCode.Backslash: return "\\";
            case KeyCode.BackQuote: return "`";
            default: return key.ToString();
        }
    }
}
