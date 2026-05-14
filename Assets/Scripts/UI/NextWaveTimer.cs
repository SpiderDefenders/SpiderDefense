using TMPro;
using UnityEngine;

public class NextWaveTimer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform timerPanel;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Animation")]
    [SerializeField] private float animationTime = 0.3f;
    [SerializeField] private float hiddenY = 200f;
    [SerializeField] private float visibleY = -50f;

    private float currentTime;
    private bool isRunning = false;

    private void OnEnable()
    {
        EventManager.Instance.OnAdditionalPathPlacingCompleted += Show;
        EventManager.Instance.OnWaveStarted += Hide;
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null) return;
        EventManager.Instance.OnAdditionalPathPlacingCompleted -= Show;
        EventManager.Instance.OnWaveStarted -= Hide;
    }

    private void Start()
    {
        Vector2 pos = timerPanel.anchoredPosition;
        pos.y = hiddenY;
        timerPanel.anchoredPosition = pos;
    }

    private void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            UpdateTimerUI();
            Invoke(nameof(AnimateOut), 1f);
            return;
        }

        UpdateTimerUI();
    }

    private void Show()
    {
        CancelInvoke(nameof(AnimateOut));
        currentTime = FindAnyObjectByType<LevelManager>().GetDelayAfterAdditionalPathPlaced();
        isRunning = true;
        UpdateTimerUI();
        AnimateIn();
    }

    private void Hide(int _)
    {
        isRunning = false;
        AnimateOut();
    }

    private void AnimateIn()
    {
        LeanTween.cancel(timerPanel);
        LeanTween.moveY(timerPanel, visibleY, animationTime)
            .setEaseInOutCubic()
            .setIgnoreTimeScale(true);
    }

    private void AnimateOut()
    {
        LeanTween.cancel(timerPanel);
        LeanTween.moveY(timerPanel, hiddenY, animationTime)
            .setEaseInOutCubic()
            .setIgnoreTimeScale(true);
    }

    private void UpdateTimerUI()
    {
        timerText.text = $"Next wave in {Mathf.CeilToInt(currentTime)}s";
    }
}