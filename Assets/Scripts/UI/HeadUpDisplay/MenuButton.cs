using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Animation Settings")]
    public float scaleUp = 1.2f;
    public float duration = 0.2f;

    private Vector3 originalScale;
    private RectTransform buttonTransform;
    private void Start()
    {
        buttonTransform = GetComponent<RectTransform>();
        originalScale = buttonTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(buttonTransform, originalScale * scaleUp, duration)   
            .setEaseOutBack()
            .setIgnoreTimeScale(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(buttonTransform, originalScale, duration)
            .setEaseOutBack()
            .setIgnoreTimeScale(true);
    }
}