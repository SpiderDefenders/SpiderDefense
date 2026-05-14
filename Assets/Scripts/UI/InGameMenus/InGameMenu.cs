using UnityEngine;

public class InGameMenu : AdditionalPathBlocker
{
    [Header("UI")]
    [SerializeField] protected RectTransform menuPanel;
    [SerializeField] protected RectTransform toggleButton;

    [Header("Animation")]
    [SerializeField] private float animationTime = 0.3f;
    [SerializeField] private float hiddenY = -300f;
    [SerializeField] private float completelyHiddenY = -400f;
    [SerializeField] protected float visibleY = -76.05f;

    [SerializeField] private bool overrideIsBlocked = false;
    protected bool isOpen = false;
    protected bool moveBackOnDown = false;

    protected void Init()
    {
        Vector2 pos = menuPanel.anchoredPosition;
        pos.y = hiddenY;
        menuPanel.anchoredPosition = pos;
    }

    private void Start()
    {
        Init();
    }

    public void ToggleMenu()
    {
        if (!overrideIsBlocked && isBlocked) isOpen = true;
        LeanTween.cancel(menuPanel);

        float targetY = isOpen ? hiddenY : visibleY;

        LeanTween.moveY(menuPanel, targetY, animationTime)
            .setEaseInOutCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                if (targetY == hiddenY && moveBackOnDown)
                {
                    menuPanel.transform.SetAsFirstSibling();
                }
            });

        isOpen = !isOpen;
    }

    public void ToggleMenu(bool desiredState)
    {
        if (isOpen == desiredState) return;
        ToggleMenu();
    }

    public void CompletelyHideMenu()
    {
        LeanTween.cancel(menuPanel);

        LeanTween.moveY(menuPanel, completelyHiddenY, animationTime)
            .setEaseInOutCubic()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                menuPanel.transform.SetAsFirstSibling();
            });

        isOpen = false;
    }

    public void RestoreHiddenMenu()
    {
        LeanTween.cancel(menuPanel);

        LeanTween.moveY(menuPanel, hiddenY, animationTime)
            .setEaseInOutCubic()
            .setIgnoreTimeScale(true);

        isOpen = false;
    }

    public virtual void CloseEverything()
    {
        ToggleMenu(false);
    }

    public bool IsOpen() { return isOpen; }
}