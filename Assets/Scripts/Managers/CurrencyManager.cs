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
    }

    private void OnDisable()
    {
        EventManager.Instance.OnEnemyDead -= AddEnemyMoney;
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

    public void AddEnemyMoney(int moneyAfterDead)
    {
        AddMoney(moneyAfterDead);
    }

    public void OnTowerRemoved(int towerValue)
    {
        AddMoney(GetMoneyOnTowerRemoved(towerValue));
    }

    public int GetCurrentMoney() { return currentAmount; }
    public int GetMoneyOnTowerRemoved(int towerValue)
    {
        return (int)(towerValue * removeMoneyFactor);
    }

}