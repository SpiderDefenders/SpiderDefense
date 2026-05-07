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
        EventManager.Instance.OnPathGenerated += StartTimer;
        EventManager.Instance.OnGameOver += StopTimer;
        EventManager.Instance.OnLevelCompleted += StopTimer;

    }

    private void OnDisable()
    {
        EventManager.Instance.OnPathGenerated -= StartTimer;
        EventManager.Instance.OnGameOver += StopTimer;
        EventManager.Instance.OnLevelCompleted += StopTimer;
    }

    private void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateTimerUI();
    }

    private void StartTimer()
    {
        elapsedTime = 0;
        isRunning = true;
        UpdateTimerUI();
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        timerText.text = $"{minutes}:{seconds:00}";
    }
}