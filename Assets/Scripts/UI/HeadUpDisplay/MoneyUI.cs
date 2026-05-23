using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour 
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI moneyText;

    private bool isAddingMoneyStopped;
    
    private void OnEnable()
    {
        EventManager.Instance.OnMoneyChanged += HandleMoneyUpdate;
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null) return;

        EventManager.Instance.OnMoneyChanged -= HandleMoneyUpdate;
    }

    private void HandleMoneyUpdate(int money)
    {
        if (isAddingMoneyStopped) return;
        moneyText.text = money.ToString();
    }
}