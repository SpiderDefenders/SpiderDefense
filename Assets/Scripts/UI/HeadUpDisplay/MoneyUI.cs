using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour 
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI moneyText;

    private bool isAddingMoneyStopped;
    
    private void OnEnable()
    {
        StartAddingMoney();
        EventManager.Instance.OnMoneyChanged += HandleMoneyUpdate;
        EventManager.Instance.OnGameOver += StopAddingMoney;
        EventManager.Instance.OnLevelCompleted += StopAddingMoney;
        EventManager.Instance.OnWaveStarted += WaveStarted;
        EventManager.Instance.OnWaveCompleted += WaveFinished;
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null) return;

        EventManager.Instance.OnMoneyChanged -= HandleMoneyUpdate;
        EventManager.Instance.OnGameOver -= StopAddingMoney;
        EventManager.Instance.OnLevelCompleted -= StopAddingMoney;
        EventManager.Instance.OnWaveStarted -= WaveStarted;
        EventManager.Instance.OnWaveCompleted -= WaveFinished;
    }

    private void WaveFinished(int _, bool isLastWave)
    {
        StopAddingMoney();
    }

    private void WaveStarted(int _)
    {
        StartAddingMoney();
    }
    
    private void StopAddingMoney()
    {
        isAddingMoneyStopped = true;
    }

    private void StartAddingMoney()
    {
        isAddingMoneyStopped = false;
    }

    private void HandleMoneyUpdate(int money)
    {
        if (isAddingMoneyStopped) return;
        moneyText.text = money.ToString();
    }
}