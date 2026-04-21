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
    public event Action OnEnemyReachedTheEnd;
    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCompleted;

    public void PathGenerated() => OnPathGenerated?.Invoke();
    public void EnemyReachedTheEnd() => OnEnemyReachedTheEnd?.Invoke();
    public void WaveStarted(int waveIndex) => OnWaveStarted?.Invoke(waveIndex);
    public void WaveCompleted(int waveIndex) => OnWaveCompleted?.Invoke(waveIndex);
}