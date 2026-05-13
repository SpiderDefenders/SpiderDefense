using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class FinishUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform container;
    public CanvasGroup darkBackground;

    public TextMeshProUGUI statusText;
    public TextMeshProUGUI scoreLabel;
    public TextMeshProUGUI scoreValue;

    public RectTransform homeButton;
    public RectTransform restartButton;
    public RectTransform reviewMapButton;

    public Button homeBtnComponent;
    public Button restartBtnComponent;
    public Button reviewMapBtnComponent;

    public RectTransform newRecord;

    [Header("Animation")]
    public float animDuration = 0.4f;
    public float overshoot = 40f;

    private Vector2 hiddenPos;
    private Vector2 centerPos;

    private bool isAnimating = false;
    private bool isNewRecord;

    private void OnEnable()
    {
        EventManager.Instance.OnLevelCompleted += ShowVictoryUI;
        EventManager.Instance.OnGameOver += ShowDefeatUI;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnLevelCompleted -= ShowVictoryUI;
        EventManager.Instance.OnGameOver -= ShowDefeatUI;
    }

    // TODO temporary solution
    private bool IsNewRecord()
    {
        return true;
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
        reviewMapButton.localScale = Vector3.zero;

        homeBtnComponent.interactable = false;
        restartBtnComponent.interactable = false;
        reviewMapBtnComponent.interactable = false;
    }

    private void ShowVictoryUI()
    {
        scoreValue.text = FindAnyObjectByType<TimerDisplay>().GetTimeString();
        scoreLabel.text = "Time";
        statusText.text = "VICTORY";
        statusText.color = Color.green;
        isNewRecord = IsNewRecord();
        Show();
    }

    private void ShowDefeatUI()
    {
        LevelManager levelManager = FindAnyObjectByType<LevelManager>();
        int round = levelManager.GetCurrentWave();
        int maxRound = levelManager.GetMaxWave();
        scoreValue.text = $"{round} <size=60>of</size> {maxRound}";
        scoreLabel.text = "Round";
        statusText.text = "DEFEAT";
        statusText.color = Color.red;
        Show();
    }


    private void PlayNewRecordStamp()
    {
        if (!isNewRecord) 
        {
            isAnimating = false;
            return;
        }

        newRecord.gameObject.SetActive(true);

        RectTransform rt = newRecord;
        Vector2 targetPos = newRecord.anchoredPosition;
        Vector3 targetScale = newRecord.localScale;

        newRecord.anchoredPosition = targetPos;
        newRecord.localScale = Vector3.one * 2.5f;
        newRecord.rotation = Quaternion.Euler(0, 0, 15f);

        LeanTween.scale(newRecord, Vector3.one * 0.95f, 0.18f)
            .setEaseInQuad()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                LeanTween.scale(rt, new Vector3(targetScale.x * 1.08f, targetScale.y * 0.78f, 1f), 0.09f)
                    .setEaseOutQuad()
                    .setIgnoreTimeScale(true)
                    .setOnComplete(() =>
                    {
                        LeanTween.scale(newRecord, targetScale, 0.22f)
                            .setEaseOutBack()
                            .setIgnoreTimeScale(true);
                        isAnimating = false;
                    });

                
                //LeanTween.rotateZ(rt.gameObject, 0f, 0.25f)
                //    .setEaseOutQuad()
                //    .setIgnoreTimeScale(true);
            });
}

    private void Show()
    {
        if (isAnimating) return;
        isAnimating = true;

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

        LeanTween.scale(reviewMapButton, Vector3.one * 1.1f, 0.25f)
                 .setDelay(delay + 0.1f)
                 .setEaseOutBack()
                 .setIgnoreTimeScale(true)
                 .setOnComplete(() =>
                 {
                     LeanTween.scale(reviewMapButton, Vector3.one, 0.1f)
                              .setIgnoreTimeScale(true);

                     reviewMapBtnComponent.interactable = true;
                 });

        LeanTween.scale(restartButton, Vector3.one * 1.1f, 0.25f)
                 .setDelay(delay + 0.2f)
                 .setEaseOutBack()
                 .setIgnoreTimeScale(true)
                 .setOnComplete(() =>
                 {
                     LeanTween.scale(restartButton, Vector3.one, 0.1f)
                              .setIgnoreTimeScale(true);

                     restartBtnComponent.interactable = true;
                     PlayNewRecordStamp();
                 });
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}