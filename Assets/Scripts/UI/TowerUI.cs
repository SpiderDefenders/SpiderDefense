using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerUI : InGameButtonUI
{
    [Header("Tower Data")]
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Image towerImage;
    [SerializeField] private Image moneyImage;

    private Color orginalColor;
    private Color orginalMoneyColor;

    private int towerCost;
    private int currentAmount;

    private void Start()
    {
        towerCost = towerPrefab.GetComponent<Tower>().GetCost();
        SetAmount(towerCost);

        orginalColor = towerImage.color;
        orginalMoneyColor = moneyImage.color;
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
        towerImage.color = CanBuyTower() ? orginalColor : Color.gray;
        moneyImage.color = CanBuyTower() ? orginalMoneyColor : Color.gray;
    }

    private bool CanBuyTower()
    {
        return currentAmount >= towerCost;
    }

}