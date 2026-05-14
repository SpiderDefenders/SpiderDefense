using TMPro;
using UnityEngine;

public class TimerDisplay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    private float elapsedTime;
    private bool isRunning = false;

    private void OnEnable()
    {
        ResetTimer();
        EventManager.Instance.OnPathGenerated += StartTimer;
        EventManager.Instance.OnGameOver += StopTimer;
        EventManager.Instance.OnLevelCompleted += StopTimer;
        EventManager.Instance.OnWaveStarted += WaveStarted;
        EventManager.Instance.OnWaveCompleted += WaveCompleted;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnPathGenerated -= StartTimer;
        EventManager.Instance.OnGameOver -= StopTimer;
        EventManager.Instance.OnLevelCompleted -= StopTimer;
        EventManager.Instance.OnWaveStarted -= WaveStarted;
        EventManager.Instance.OnWaveCompleted -= WaveCompleted;
    }

    private void WaveCompleted(int _, bool isLastWave)
    {
        StopTimer();
    }

    private void WaveStarted(int _)
    {
        StartTimer();
    }

    private void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateTimerUI();
    }

    private void StartTimer()
    {
        isRunning = true;
        UpdateTimerUI();
    }

    private void ResetTimer()
    {
        elapsedTime = 0;
        UpdateTimerUI();
    }

    private void StopTimer()
    {
        isRunning = false;
    }

    public string GetTimeString()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        return $"{minutes}:{seconds:00}";
    }

    private void UpdateTimerUI()
    {
        timerText.text = GetTimeString();
    }
}