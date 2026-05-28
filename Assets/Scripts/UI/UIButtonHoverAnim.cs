using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class UIButtonHoverAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public RectTransform textTransform;

    [Header("Animation Settings")]
    public float scaleUp = 1.2f;
    public float duration = 0.2f;

    [Header("Text Movement")]
    public Vector2 direction = Vector2.down;
    public float moveDistance = 50f;

    private Vector3 originalScale;
    private Vector2 textStartPos;
    private CanvasGroup textCanvasGroup;
    private RectTransform buttonTransform;

    void Start()
    {
        buttonTransform = GetComponent<RectTransform>();
        textCanvasGroup = textTransform.GetComponent<CanvasGroup>();
        originalScale = buttonTransform.localScale;
        textStartPos = textTransform.anchoredPosition;

        textCanvasGroup.alpha = 0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(buttonTransform, originalScale * scaleUp, duration)
            .setEaseOutBack()
            .setIgnoreTimeScale(true);

        LeanTween.alphaCanvas(textCanvasGroup, 1f, duration).setIgnoreTimeScale(true);

        Vector2 targetPos = textStartPos + direction.normalized * moveDistance;
        LeanTween.move(textTransform, targetPos, duration)
            .setEaseOutCubic()
            .setIgnoreTimeScale(true);

        LeanTween.scale(textTransform, Vector3.one * 1.1f, duration)
            .setIgnoreTimeScale(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(buttonTransform, originalScale, duration)
            .setEaseOutBack().setIgnoreTimeScale(true);

        LeanTween.alphaCanvas(textCanvasGroup, 0f, duration).setIgnoreTimeScale(true);

        LeanTween.move(textTransform, textStartPos, duration)
            .setEaseInCubic().setIgnoreTimeScale(true);

        LeanTween.scale(textTransform, Vector3.one, duration).setIgnoreTimeScale(true);
    }
}