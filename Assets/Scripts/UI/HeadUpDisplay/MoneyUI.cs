using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour 
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI moneyText;

    private bool isGameFinished;
    
    private void OnEnable()
    {
        isGameFinished = false;
        EventManager.Instance.OnMoneyChanged += HandleMoneyUpdate;
        EventManager.Instance.OnGameOver += StopAddingMoney;
        EventManager.Instance.OnLevelCompleted += StopAddingMoney;
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null) return;

        EventManager.Instance.OnMoneyChanged -= HandleMoneyUpdate;
        EventManager.Instance.OnGameOver -= StopAddingMoney;
        EventManager.Instance.OnLevelCompleted -= StopAddingMoney;
    }
    
    private void StopAddingMoney()
    {
        isGameFinished = true;
    }


    private void HandleMoneyUpdate(int money)
    {
        if (isGameFinished) return;
        moneyText.text = money.ToString();
    }
}