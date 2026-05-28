using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [SerializeField] private int startAmount = 200;
    [SerializeField] private float removeMoneyFactor = 0.8f;
    [SerializeField] private int moneyPerInterval = 50;
    [SerializeField] private float intervalSecond = 5f;

    private int currentAmount;
    private float currentInterval = 0f;

    private bool isAddingMoneyStopped = false;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentAmount = startAmount;
        EventManager.Instance.MoneyChanged(currentAmount);
    }

    private void Update()
    {
        if (isAddingMoneyStopped) return;

        if (currentInterval >= intervalSecond)
        {
            AddMoney(moneyPerInterval);
            currentInterval = 0;
        }
        currentInterval += Time.deltaTime;
    }

    private void OnEnable()
    {
        EventManager.Instance.OnEnemyDead += AddEnemyMoney;

        EventManager.Instance.OnGameOver += StopAddingMoney;
        EventManager.Instance.OnLevelCompleted += StopAddingMoney;
        EventManager.Instance.OnWaveStarted += WaveStarted;
        EventManager.Instance.OnWaveCompleted += WaveFinished;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnEnemyDead -= AddEnemyMoney;

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

    private void AddMoney(int addValue)
    {
        currentAmount += addValue;
        EventManager.Instance.MoneyChanged(currentAmount);
    }

    public void RemoveMoney(int removeValue) 
    { 
        currentAmount -= removeValue;
        EventManager.Instance.MoneyChanged(currentAmount);
    }

    public void AddEnemyMoney(int moneyAfterDead, GameObject enemyObject)
    {
        AddMoney(moneyAfterDead);
    }

    public void OnTowerRemoved(int towerValue)
    {
        AddMoney(GetMoneyOnTowerRemoved(towerValue));
    }

    public int GetMoneyOnTowerRemoved(int towerValue)
    {
        return (int)(towerValue * removeMoneyFactor);
    }
    
    public int GetCurrentAmount() => currentAmount;

}