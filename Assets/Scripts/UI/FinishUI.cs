using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinishUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform container;
    public CanvasGroup darkBackground;

    public TextMeshProUGUI scoreLabel;
    public TextMeshProUGUI scoreValue;

    public RectTransform homeButton;
    public RectTransform restartButton;

    public Button homeBtnComponent;
    public Button restartBtnComponent;

    [Header("Animation")]
    public float animDuration = 0.4f;
    public float overshoot = 40f;

    private Vector2 hiddenPos;
    private Vector2 centerPos;

    private bool isAnimating = false;

    private void OnEnable()
    {
        EventManager.Instance.OnLevelCompleted += Show;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnLevelCompleted -= Show;
    }

    void Start()
    {
        Canvas.ForceUpdateCanvases();

        centerPos = container.anchoredPosition;
        hiddenPos = centerPos + Vector2.down * Screen.height;

        container.anchoredPosition = hiddenPos;

        darkBackground.alpha = 0f;
        darkBackground.gameObject.SetActive(false);

        container.gameObject.SetActive(false);

        scoreLabel.alpha = 0f;
        scoreValue.alpha = 0f;

        homeButton.localScale = Vector3.zero;
        restartButton.localScale = Vector3.zero;

        homeBtnComponent.interactable = false;
        restartBtnComponent.interactable = false;
    }

    public void Show()
    {
        if (isAnimating) return;
        isAnimating = true;
        scoreValue.text = FindAnyObjectByType<CurrencyManager>().GetCurrentAmount().ToString();

        LeanTween.delayedCall(gameObject, 3f, () =>
        {

            LeanTween.cancel(gameObject);

            darkBackground.gameObject.SetActive(true);
            container.gameObject.SetActive(true);

            LeanTween.alphaCanvas(darkBackground, 0.5f, animDuration)
                .setIgnoreTimeScale(true);

            container.anchoredPosition = hiddenPos;

            LeanTween.move(container, centerPos + Vector2.up * overshoot, animDuration * 0.7f)
                .setEaseOutCubic()
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    LeanTween.move(container, centerPos, animDuration * 0.3f)
                        .setEaseInOutQuad()
                        .setIgnoreTimeScale(true)
                        .setOnComplete(PlayContentAnimation);
                });
        }).setIgnoreTimeScale(true);
    }

    private void PlayContentAnimation()
    {
        LeanTween.value(gameObject, 0f, 1f, 0.3f)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float val) =>
            {
                var c = scoreLabel.color;
                scoreLabel.color = new Color(c.r, c.g, c.b, val);
            });

        LeanTween.value(gameObject, 0f, 1f, 0.3f)
            .setDelay(0.2f)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float val) =>
            {
                var c = scoreValue.color;
                scoreValue.color = new Color(c.r, c.g, c.b, val);
            });

        float delay = 0.4f;

        LeanTween.scale(homeButton, Vector3.one * 1.1f, 0.25f)
                 .setDelay(delay)
                 .setEaseOutBack()
                 .setIgnoreTimeScale(true)
                 .setOnComplete(() =>
                 {
                     LeanTween.scale(homeButton, Vector3.one, 0.1f)
                              .setIgnoreTimeScale(true);

                     homeBtnComponent.interactable = true;
                 });

        LeanTween.scale(restartButton, Vector3.one * 1.1f, 0.25f)
                 .setDelay(delay + 0.1f)
                 .setEaseOutBack()
                 .setIgnoreTimeScale(true)
                 .setIgnoreTimeScale(true)
                 .setOnComplete(() =>
                 {
                     LeanTween.scale(restartButton, Vector3.one, 0.1f)
                              .setIgnoreTimeScale(true);

                     restartBtnComponent.interactable = true;
                     isAnimating = false;
                 });
    }
}