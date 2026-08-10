using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerUI : InGameButtonUI
{
    [Header("Tower Data")]
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Image towerImage;
    [SerializeField] private Image moneyImage;

    private Color orginalColor = Color.white;
    private Color unavailableColor = Color.gray;

    private int towerCost;
    private int currentAmount;

    private void Start()
    {
        towerCost = towerPrefab.GetComponent<Tower>().Cost;
        SetAmount(towerCost);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnMoneyChanged += HandleMoneyUpdate;
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null) return;

        EventManager.Instance.OnMoneyChanged -= HandleMoneyUpdate;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanBuyTower()) return;
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!CanBuyTower()) return;

        base.OnPointerClick(eventData);
        BuildManager.Instance.StartPlacement(towerPrefab);
    }

    private void HandleMoneyUpdate(int money)
    {
        currentAmount = money;
        bool canBuy = CanBuyTower();
        towerImage.color = GetColor(canBuy);
        moneyImage.color = GetColor(canBuy);
        amountText.color = GetColor(canBuy);
    }

    private Color GetColor(bool standardColor)
    {
        return standardColor ? orginalColor : unavailableColor;
    }

    private bool CanBuyTower()
    {
        return currentAmount >= towerCost;
    }

}