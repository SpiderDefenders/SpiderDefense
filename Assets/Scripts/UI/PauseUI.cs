using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public RectTransform pauseContainer;
    public CanvasGroup darkBackground;

    [Header("Animation")]
    public float animDuration = 0.4f;
    public float overshoot = 40f;

    private Vector2 hiddenPos;
    private Vector2 centerPos;
    
    private GameManager gameManager;
    private MenuManager menuManager;

    private bool isPaused = false;
    private bool isAnimating = false;
    
    void OnEnable()
    {
        InputHandler.OnPausePressed += TogglePause;
    }

    void OnDisable()
    {
        InputHandler.OnPausePressed -= TogglePause;
    }

    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        menuManager = FindAnyObjectByType<MenuManager>();
        Canvas.ForceUpdateCanvases();
        centerPos = pauseContainer.anchoredPosition;

        float screenOffset = Screen.height;
        hiddenPos = centerPos + Vector2.down * screenOffset;

        pauseContainer.anchoredPosition = hiddenPos;
        darkBackground.alpha = 0f;
        darkBackground.gameObject.SetActive(false);
        pauseContainer.gameObject.SetActive(false);
    }

    public void TogglePause()
    {
        if (isAnimating) return;

        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        isPaused = true;
        isAnimating = true;

        gameManager.PauseGame();
        menuManager.CloseAll();

        AudioManager.Instance.StopMusicWithFade(0.5f);
        
        LeanTween.cancel(pauseContainer);
        LeanTween.cancel(darkBackground.gameObject);

        darkBackground.gameObject.SetActive(true);
        pauseContainer.gameObject.SetActive(true);

        LeanTween.alphaCanvas(darkBackground, 0.5f, animDuration)
                 .setIgnoreTimeScale(true);

        pauseContainer.anchoredPosition = hiddenPos;

        LeanTween.move(pauseContainer, centerPos + Vector2.up * overshoot, animDuration * 0.7f)
                 .setEaseOutCubic()
                 .setIgnoreTimeScale(true)
                 .setOnComplete(() =>
                 {
                     LeanTween.move(pauseContainer, centerPos, animDuration * 0.3f)
                              .setEaseInOutQuad()
                              .setIgnoreTimeScale(true)
                              .setOnComplete(() =>
                              {
                                  isAnimating = false;
                              });
                 });
    }

    public void Resume()
    {
        isPaused = false;
        isAnimating = true;

        gameManager.ResumeGame();

        AudioManager.Instance.ResumeMusicWithFade(0.5f);
        
        LeanTween.cancel(pauseContainer);
        LeanTween.cancel(darkBackground.gameObject);

        LeanTween.alphaCanvas(darkBackground, 0f, animDuration)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                darkBackground.gameObject.SetActive(false);
            });

        LeanTween.move(pauseContainer, centerPos + Vector2.up * overshoot, animDuration * 0.3f)
            .setEaseOutCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                LeanTween.move(pauseContainer, hiddenPos, animDuration * 0.7f)
                    .setEaseInCubic()
                    .setIgnoreTimeScale(true)
                    .setOnComplete(() =>
                    {
                        pauseContainer.gameObject.SetActive(false);
                        isAnimating = false;
                    });
            });
        EventManager.Instance.GameResumed();
    }
}