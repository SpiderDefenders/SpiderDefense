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
    public event Action<int> OnEnemyReachedTheEnd;
    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCompleted;
    public event Action OnGameOver;

    public void PathGenerated() => OnPathGenerated?.Invoke();
    public void EnemyReachedTheEnd(int damage) => OnEnemyReachedTheEnd?.Invoke(damage);
    public void WaveStarted(int waveIndex) => OnWaveStarted?.Invoke(waveIndex);
    public void WaveCompleted(int waveIndex) => OnWaveCompleted?.Invoke(waveIndex);
    public void GameOver() => OnGameOver?.Invoke();
}