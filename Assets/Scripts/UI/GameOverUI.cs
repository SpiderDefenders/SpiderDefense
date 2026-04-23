using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI restartText;
    [SerializeField] private TextMeshProUGUI quitText;
    [SerializeField] private Image backgroundPanel;

    [Header("Animation")]
    [SerializeField] private float moveDistance = 300f;
    [SerializeField] private float animationTime = 1.2f;
    [SerializeField] private float buttonsDelay = 3f;
    [SerializeField] private float buttonFadeTime = 0.5f;

    private RectTransform rectTransform;
    private CanvasGroup textCanvasGroup;
    private CanvasGroup restartCanvasGroup;
    private CanvasGroup quitCanvasGroup;
    private CanvasGroup bgCanvasGroup;

    private Button restartButton;
    private Button quitButton;

    private Vector2 centerPos;

    private void Awake()
    {
        gameOverText.gameObject.SetActive(true);
        restartText.gameObject.SetActive(true);
        quitText.gameObject.SetActive(true);
        backgroundPanel.gameObject.SetActive(true);
        rectTransform = gameOverText.GetComponent<RectTransform>();
        centerPos = rectTransform.anchoredPosition;

        textCanvasGroup = GetOrAddCanvasGroup(gameOverText);
        restartCanvasGroup = GetOrAddCanvasGroup(restartText);
        quitCanvasGroup = GetOrAddCanvasGroup(quitText);
        bgCanvasGroup = GetOrAddCanvasGroup(backgroundPanel);

        restartButton = GetOrAddButton(restartText);
        quitButton = GetOrAddButton(quitText);

        restartButton.onClick.AddListener(Restart);
        quitButton.onClick.AddListener(Quit);

        restartButton.interactable = false;
        quitButton.interactable = false;

        textCanvasGroup.alpha = 0;
        restartCanvasGroup.alpha = 0;
        quitCanvasGroup.alpha = 0;
        bgCanvasGroup.alpha = 0;
        gameOverText.gameObject.SetActive(false);
        restartText.gameObject.SetActive(false);
        quitText.gameObject.SetActive(false);
        backgroundPanel.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnGameOver += ShowGameOver;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnGameOver -= ShowGameOver;
    }

    private CanvasGroup GetOrAddCanvasGroup(Component obj)
    {
        var cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = obj.gameObject.AddComponent<CanvasGroup>();
        return cg;
    }

    private Button GetOrAddButton(TextMeshProUGUI txt)
    {
        var btn = txt.GetComponent<Button>();
        if (btn == null)
            btn = txt.gameObject.AddComponent<Button>();
        return btn;
    }

    private void ShowGameOver()
    {
        gameOverText.gameObject.SetActive(true);
        restartText.gameObject.SetActive(true);
        quitText.gameObject.SetActive(true);
        backgroundPanel.gameObject.SetActive(true);
        LeanTween.alphaCanvas(bgCanvasGroup, 0.6f, 1f);

        rectTransform.anchoredPosition = centerPos - new Vector2(0, moveDistance);
        textCanvasGroup.alpha = 0;

        LeanTween.alphaCanvas(textCanvasGroup, 1f, animationTime);
        LeanTween.moveY(rectTransform, centerPos.y, animationTime)
            .setEaseOutCubic();

        gameOverText.transform.localScale = Vector3.one * 0.8f;
        LeanTween.scale(gameOverText.gameObject, Vector3.one, animationTime)
            .setEaseOutBack();

        LeanTween.delayedCall(buttonsDelay, ShowButtons);
    }

    private void ShowButtons()
    {
        LeanTween.alphaCanvas(restartCanvasGroup, 1f, buttonFadeTime)
            .setOnComplete(() =>
            {
                restartButton.interactable = true;

                LeanTween.alphaCanvas(quitCanvasGroup, 1f, buttonFadeTime)
                    .setOnComplete(() =>
                    {
                        quitButton.interactable = true;
                    });
            });
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Quit()
    {
        Application.Quit();
    }
}