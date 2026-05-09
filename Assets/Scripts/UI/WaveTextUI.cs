using UnityEngine;
using TMPro;

public class WaveTextUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI text;

    [Header("Animation")]
    [SerializeField] private float showDuration = 1.5f;
    [SerializeField] private float moveDistance = 50f;
    [SerializeField] private float animationTime = 0.5f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startPos;

    private void Awake()
    {
        rectTransform = text.GetComponent<RectTransform>();

        canvasGroup = text.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = text.gameObject.AddComponent<CanvasGroup>();

        startPos = rectTransform.anchoredPosition;

        canvasGroup.alpha = 0;
    }

    private void OnEnable()
    {
        EventManager.Instance.OnWaveStarted += HandleWaveStarted;
        EventManager.Instance.OnWaveCompleted += HandleWaveCompleted;
        EventManager.Instance.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null) return;

        EventManager.Instance.OnWaveStarted -= HandleWaveStarted;
        EventManager.Instance.OnWaveCompleted -= HandleWaveCompleted;
        EventManager.Instance.OnGameOver -= HandleGameOver;
    }

    private void HandleWaveStarted(int waveIndex)
    {
        ShowText($"Wave {waveIndex}");
    }

    private void HandleWaveCompleted(int waveIndex, bool _)
    {
        ShowText($"Wave {waveIndex} Completed");
    }

    private void ShowText(string message)
    {
        text.text = message;

        LeanTween.cancel(text.gameObject);

        rectTransform.anchoredPosition = startPos - new Vector2(0, moveDistance);
        canvasGroup.alpha = 0;

        LeanTween.alphaCanvas(canvasGroup, 1f, animationTime);
        LeanTween.moveY(rectTransform, startPos.y, animationTime)
            .setEaseOutBack();

        LeanTween.delayedCall(showDuration, () =>
        {
            LeanTween.alphaCanvas(canvasGroup, 0f, animationTime);
            LeanTween.moveY(rectTransform, startPos.y + moveDistance, animationTime)
                .setEaseInBack();
        });
    }
    
    private void HandleGameOver()
    {
        LeanTween.cancel(text.gameObject);
        LeanTween.cancel(rectTransform);

        LeanTween.cancel(gameObject);

        canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha);

        LeanTween.alphaCanvas(canvasGroup, 0f, 0.1f);

        LeanTween.scale(text.gameObject, Vector3.one * 0.9f, 0.1f)
            .setEaseOutQuad();

        LeanTween.delayedCall(0.1f, () =>
        {
            gameObject.SetActive(false);
        });
    }
}