using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Components")]
    [SerializeField] private Image borderImage;
    [SerializeField] private Image image;
    [SerializeField] private GameObject moneyUI;

    private Image coinImage;
    private TMP_Text costText;

    [Header("Animation Settings")]
    [SerializeField] private float scaleUp = 1.2f;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private RectTransform animationTransform;

    private Vector3 originalScale;

    private Color orginalColor = Color.white;
    private Color unavailableColor = Color.gray;

    private bool isPurchased = false;
    private bool canBuy = true;
    private int cost = 0;
    private int currentAmount;
    private int upgradeIdx = 0;

    private TowerMenu towerMenu;

    private void Start()
    {
        coinImage = moneyUI.GetComponentInChildren<Image>();
        costText = moneyUI.GetComponentInChildren<TMP_Text>();
        towerMenu = FindAnyObjectByType<TowerMenu>();

        originalScale = animationTransform.localScale;
    }

    private void OnEnable()
    {
        EventManager.Instance.OnMoneyChanged += HandleMoneyUpdate;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnMoneyChanged -= HandleMoneyUpdate;
    }

    public void SetTowerData(UpgradeSO upgradeSO, int idx, bool isPurchased)
    {
        image.sprite = upgradeSO.upgardeImage;
        costText.text = upgradeSO.cost.ToString();
        cost = upgradeSO.cost;
        upgradeIdx = idx;
        this.isPurchased = isPurchased;

        SetPurchasedView();
        HandleMoneyUpdate(currentAmount);       
    }

    private void HandleMoneyUpdate(int money)
    {
        currentAmount = money;
        if (isPurchased) return;

        canBuy = CanBuyTower();
        borderImage.color = GetColor(canBuy);
        coinImage.color = GetColor(canBuy);
        image.color = GetColor(canBuy);
        costText.color = GetColor(canBuy);
    }

    private bool CanBuyTower()
    {
        return currentAmount >= cost;
    }

    private Color GetColor(bool standardColor)
    {
        return standardColor ? orginalColor : unavailableColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isPurchased && canBuy)
        {
            LeanTween.scale(animationTransform, originalScale * scaleUp, duration)
            .setEaseOutBack()
            .setIgnoreTimeScale(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(animationTransform, originalScale, duration)
            .setEaseOutBack()
            .setIgnoreTimeScale(true);
    }

    private void SetPurchasedView()
    {
        if (isPurchased)
        {
            ColorUtility.TryParseHtmlString("#00BF63", out Color color);
            borderImage.color = color;
            image.color = orginalColor;

            moneyUI.SetActive(false);
        }
        else 
        { 
            moneyUI.SetActive(true);
            borderImage.color = orginalColor;
        }
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isPurchased && canBuy)
        {
            isPurchased = true;
            
            towerMenu.UpdateTower(upgradeIdx);
            CurrencyManager.Instance.RemoveMoney(cost);

            LeanTween.cancel(animationTransform);
            // Click animation sequence
            LeanTween.scale(animationTransform, animationTransform.localScale * 1.2f, duration / 2)
                .setEaseInOutQuad()
                .setOnComplete(() =>
                {
                    LeanTween.scale(animationTransform, originalScale * scaleUp, duration / 2)
                        .setEaseOutBack();
                    SetPurchasedView();
                });
        }
    }

}