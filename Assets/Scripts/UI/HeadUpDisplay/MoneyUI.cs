using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour 
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI moneyText;

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
        moneyText.text = money.ToString();
    }
}