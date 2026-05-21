using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform settingsContainer;
    public CanvasGroup darkBackground;
    public RectTransform pauseContainer;

    [Header("Animation")]
    public float animDuration = 0.4f;
    public float overshoot = 40f;

    [Header("Audio Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private Vector2 hiddenPos;
    private Vector2 centerPos;

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        centerPos = settingsContainer.anchoredPosition;
        float screenOffset = Screen.height;
        hiddenPos = centerPos + Vector2.down * screenOffset;
        settingsContainer.anchoredPosition = hiddenPos;
        settingsContainer.gameObject.SetActive(false);

        InitSliders();
        AddListeners();
    }

    private void OnEnable()
    {
        if (AudioManager.Instance == null) return;
        InitSliders();
        AddListeners();
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    private void InitSliders()
    {
        masterSlider.SetValueWithoutNotify(AudioManager.Instance.GetMasterVolume());
        musicSlider.SetValueWithoutNotify(AudioManager.Instance.GetMusicVolume());
        sfxSlider.SetValueWithoutNotify(AudioManager.Instance.GetSFXVolume());
    }

    private void AddListeners()
    {
        RemoveListeners();
        masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
    }

    private void RemoveListeners()
    {
        if (AudioManager.Instance == null) return;
        masterSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetMasterVolume);
        musicSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetSFXVolume);
    }

    public void OpenSettings()
    {
        LeanTween.cancel(pauseContainer);

        LeanTween.move(pauseContainer, pauseContainer.anchoredPosition + Vector2.down * Screen.height, animDuration * 0.7f)
            .setEaseInCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(() => { pauseContainer.gameObject.SetActive(false); });

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
        LeanTween.cancel(settingsContainer);

        LeanTween.move(settingsContainer, centerPos + Vector2.up * overshoot, animDuration * 0.3f)
            .setEaseOutCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                LeanTween.move(settingsContainer, hiddenPos, animDuration * 0.7f)
                    .setEaseInCubic()
                    .setIgnoreTimeScale(true)
                    .setOnComplete(() => { settingsContainer.gameObject.SetActive(false); });
            });

        pauseContainer.anchoredPosition = pauseContainer.anchoredPosition + Vector2.down * Screen.height;
        pauseContainer.gameObject.SetActive(true);

        LeanTween.move(pauseContainer, GetPauseCenterPos(), animDuration * 0.7f)
            .setEaseOutCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                LeanTween.move(pauseContainer, GetPauseCenterPos(), animDuration * 0.3f)
                    .setEaseInOutQuad()
                    .setIgnoreTimeScale(true);
            });
    }

    private Vector2 GetPauseCenterPos()
    {
        return new Vector2(pauseContainer.anchoredPosition.x, 0f);
    }
}