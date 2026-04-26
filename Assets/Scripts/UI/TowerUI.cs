using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerUI : InGameButtonUI
{
    [Header("Tower Data")]
    [SerializeField] private GameObject towerPrefab;
    private Image imageComponent;
    private Color orginalColor;

    private int towerCost;
    private int currentAmount;

    private void Start()
    {
        towerCost = towerPrefab.GetComponent<Tower>().GetCost();
        SetAmount(towerCost);

        imageComponent = image.GetComponent<Image>();
        orginalColor = imageComponent.color;
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
        imageComponent.color = CanBuyTower() ? orginalColor : Color.gray;
    }

    private bool CanBuyTower()
    {
        return currentAmount >= towerCost;
    }

}