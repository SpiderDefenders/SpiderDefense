using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    private const string CAMERA_SPEED_KEY = "CameraSpeed";

    [Header("References")]
    public RectTransform settingsContainer;
    public CanvasGroup darkBackground;

    [Header("Animation")]
    public float animDuration = 0.4f;
    public float overshoot = 40f;

    [Header("Audio Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Camera Sliders")]
    public Slider cameraSpeedSlider;
    public SimpleRtsCamera.Scripts.SimpleRtsCamera rtsCamera;

    private Vector2 hiddenPos;
    private Vector2 centerPos;
    private RectTransform _callerContainer;
    
    private Vector2 _callerOriginalPos;

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        centerPos = settingsContainer.anchoredPosition;
        hiddenPos = centerPos + Vector2.down * Screen.height;
        settingsContainer.anchoredPosition = hiddenPos;
        settingsContainer.gameObject.SetActive(false);

        LoadSettings();
    }

    private void OnEnable()
    {
        InitSliders();
        AddListeners();
    }

    private void InitSliders()
    {
        masterSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(AudioManager.PrefMaster, 1f));
        musicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(AudioManager.PrefMusic, 1f));
        sfxSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(AudioManager.PrefSFX, 1f));
        cameraSpeedSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(CAMERA_SPEED_KEY, 200f));
    }

    private void AddListeners()
    {
        RemoveListeners();
        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        cameraSpeedSlider.onValueChanged.AddListener(OnCameraSpeedChanged);
    }

    private void RemoveListeners()
    {
        masterSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        cameraSpeedSlider.onValueChanged.RemoveListener(OnCameraSpeedChanged);
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    private void LoadSettings()
    {
        if (AudioManager.Instance == null) return;

        float master = PlayerPrefs.GetFloat(AudioManager.PrefMaster, 1f);
        float music = PlayerPrefs.GetFloat(AudioManager.PrefMusic, 1f);
        float sfx = PlayerPrefs.GetFloat(AudioManager.PrefSFX, 1f);
        float cameraSpeed = PlayerPrefs.GetFloat(CAMERA_SPEED_KEY, rtsCamera != null ? rtsCamera.MoveSpeed : 10f);

        AudioManager.Instance.SetMasterVolume(master);
        AudioManager.Instance.SetMusicVolume(music);
        AudioManager.Instance.SetSFXVolume(sfx);

        if (rtsCamera != null)
            rtsCamera.MoveSpeed = cameraSpeed;
    }
    
    private void OnMasterVolumeChanged(float value) => AudioManager.Instance.SetMasterVolume(value);
    private void OnMusicVolumeChanged(float value)  => AudioManager.Instance.SetMusicVolume(value);
    private void OnSFXVolumeChanged(float value)    => AudioManager.Instance.SetSFXVolume(value);

    private void OnCameraSpeedChanged(float value)
    {
        if (rtsCamera != null)
            rtsCamera.MoveSpeed = value;

        PlayerPrefs.SetFloat(CAMERA_SPEED_KEY, value);
        PlayerPrefs.Save();
    }
        
    public void OpenSettings(RectTransform callerContainer)
    {
        _callerContainer = callerContainer;
        _callerOriginalPos = callerContainer.anchoredPosition;

        LeanTween.cancel(callerContainer);
        LeanTween.move(callerContainer, callerContainer.anchoredPosition + Vector2.down * Screen.height, animDuration * 0.7f)
            .setEaseInCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(() => callerContainer.gameObject.SetActive(false));

        settingsContainer.anchoredPosition = hiddenPos;
        settingsContainer.gameObject.SetActive(true);

        LeanTween.move(settingsContainer, centerPos + Vector2.up * overshoot, animDuration * 0.7f)
            .setEaseOutCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                LeanTween.move(settingsContainer, centerPos, animDuration * 0.3f)
                    .setEaseInOutQuad()
                    .setIgnoreTimeScale(true);
            });
    }

    public void CloseSettings()
    {
        if (_callerContainer == null) return;

        LeanTween.cancel(settingsContainer);
        LeanTween.move(settingsContainer, centerPos + Vector2.up * overshoot, animDuration * 0.3f)
            .setEaseOutCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                LeanTween.move(settingsContainer, hiddenPos, animDuration * 0.7f)
                    .setEaseInCubic()
                    .setIgnoreTimeScale(true)
                    .setOnComplete(() => settingsContainer.gameObject.SetActive(false));
            });

        _callerContainer.anchoredPosition = _callerOriginalPos + Vector2.down * Screen.height;
        _callerContainer.gameObject.SetActive(true);

        LeanTween.move(_callerContainer, _callerOriginalPos, animDuration * 0.7f)
            .setEaseOutCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                LeanTween.move(_callerContainer, _callerOriginalPos, animDuration * 0.3f)
                    .setEaseInOutQuad()
                    .setIgnoreTimeScale(true);
            });
    }
}