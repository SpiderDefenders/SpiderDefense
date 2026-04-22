using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TowerUI : InGameButtonUI
{
    [Header("Tower Data")]
    [SerializeField] private GameObject towerPrefab;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI amountText;

    public override void OnPointerClick(PointerEventData eventData)
    {
        
        base.OnPointerClick(eventData);
        BuildManager.Instance.StartPlacement(towerPrefab);
    }

    public void SetAmount(int amount)
    {
        amountText.text = amount.ToString();
    }
}