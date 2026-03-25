using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TowerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Tower Data")]
    [SerializeField] private GameObject towerPrefab;

    [Header("UI")]
    [SerializeField] private RectTransform towerImage;
    [SerializeField] private TextMeshProUGUI amountText;

    [Header("Animation")]
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float animationTime = 0.2f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = towerImage.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.cancel(towerImage);
        LeanTween.scale(towerImage, originalScale * hoverScale, animationTime)
            .setEaseOutBack();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(towerImage);
        LeanTween.scale(towerImage, originalScale, animationTime)
            .setEaseInOutQuad();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        LeanTween.cancel(towerImage);

        // Click animation sequence
        LeanTween.scale(towerImage, towerImage.localScale * 1.2f, animationTime / 2 )
            .setEaseInOutQuad()
            .setOnComplete(() =>
            {
                LeanTween.scale(towerImage, originalScale * hoverScale, animationTime / 2 )
                    .setEaseOutBack();
            });

        BuildManager.Instance.StartPlacement(towerPrefab);
    }

    public void SetAmount(int amount)
    {
        amountText.text = amount.ToString();
    }
}