using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public event Action OnPathGenerated;
    public event Action OnAdditionalPathPlacingStarted;
    public event Action<int> OnEnemyReachedTheEnd;
    public event Action<int> OnWaveStarted;
    public event Action<int, bool> OnWaveCompleted;
    public event Action OnGameOver;
    public event Action<int> OnEnemyDead;
    public event Action OnLevelCompleted;

    public event Action<int> OnMoneyChanged;
    public event Action<int> OnHPChanged;
    public event Action OnAdditionalPathPlacingCompleted;

    public void PathGenerated() => OnPathGenerated?.Invoke();
    public void EnemyReachedTheEnd(int damage) => OnEnemyReachedTheEnd?.Invoke(damage);
    public void WaveStarted(int waveIndex) => OnWaveStarted?.Invoke(waveIndex);
    public void WaveCompleted(int waveIndex, bool isLastWave) => OnWaveCompleted?.Invoke(waveIndex, isLastWave);
    public void GameOver() => OnGameOver?.Invoke();
    public void EnemyDead(int moneyAfterDead) => OnEnemyDead?.Invoke(moneyAfterDead);
    public void LevelCompleted() => OnLevelCompleted?.Invoke();
    public void MoneyChanged(int moneyAmount) => OnMoneyChanged?.Invoke(moneyAmount);
    public void HPChanged(int hp) => OnHPChanged?.Invoke(hp);
    public void AdditionalPathPlacingStarted() => OnAdditionalPathPlacingStarted?.Invoke();
    public void AdditionalPathPlacingCompleted() => OnAdditionalPathPlacingCompleted?.Invoke();
}