using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InGameButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    [Header("UI")]
    [SerializeField] protected RectTransform image;
    [SerializeField] protected TextMeshProUGUI amountText;

    [Header("Animation")]
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float animationTime = 0.2f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = image.localScale;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.cancel(image);
        LeanTween.scale(image, originalScale * hoverScale, animationTime)
            .setEaseOutBack();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(image);
        LeanTween.scale(image, originalScale, animationTime)
            .setEaseInOutQuad();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        LeanTween.cancel(image);

        // Click animation sequence
        LeanTween.scale(image, image.localScale * 1.2f, animationTime / 2 )
            .setEaseInOutQuad()
            .setOnComplete(() =>
            {
                LeanTween.scale(image, originalScale * hoverScale, animationTime / 2 )
                    .setEaseOutBack();
            });
    }

    public void SetAmount(int amount)
    {
        amountText.text = amount.ToString();
    }
}