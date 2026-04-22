using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class RemoveTowerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //[Header("Tower Data")]
    //[SerializeField] private GameObject towerPrefab;

    [Header("UI")]
    [SerializeField] private RectTransform image;
    //[SerializeField] private TextMeshProUGUI amountText;

    [Header("Animation")]
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float animationTime = 0.2f;

    private Vector3 originalScale;

    //private void Awake()
    //{
    //    originalScale = towerImage.localScale;
    //}

    public void OnPointerEnter(PointerEventData eventData)
    {
        //LeanTween.cancel(image);
        //LeanTween.scale(image, originalScale * hoverScale, animationTime)
        //    .setEaseOutBack();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //LeanTween.cancel(image);
        //LeanTween.scale(image, originalScale, animationTime)
        //    .setEaseInOutQuad();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //LeanTween.cancel(image);

        //// Click animation sequence
        //LeanTween.scale(image, image.localScale * 1.2f, animationTime / 2 )
        //    .setEaseInOutQuad()
        //    .setOnComplete(() =>
        //    {
        //        LeanTween.scale(image, originalScale * hoverScale, animationTime / 2 )
        //            .setEaseOutBack();
        //    });

        //BuildManager.Instance.StartPlacement(towerPrefab);
    }
}