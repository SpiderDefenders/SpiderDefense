using UnityEngine;

public class BuildingMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform menuPanel;
    [SerializeField] private RectTransform toggleButton;

    [Header("Animation")]
    [SerializeField] private float animationTime = 0.3f;
    [SerializeField] private float hiddenY = -300f; // off-screen
    [SerializeField] private float visibleY = 0f;

    private bool isOpen = false;

    private void Start()
    {
        // Start hidden
        Vector2 pos = menuPanel.anchoredPosition;
        pos.y = hiddenY;
        menuPanel.anchoredPosition = pos;
    }

    public void ToggleMenu()
    {
        LeanTween.cancel(menuPanel);

        float targetY = isOpen ? hiddenY : visibleY;

        LeanTween.moveY(menuPanel, targetY, animationTime)
            .setEaseInOutCubic();

        isOpen = !isOpen;
    }
}