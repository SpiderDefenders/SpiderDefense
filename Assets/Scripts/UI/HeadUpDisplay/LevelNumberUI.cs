using TMPro;
using UnityEngine;

public class LevelNumberUI : MonoBehaviour 
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI levelText;

    private void OnEnable()
    {
        EventManager.Instance.OnWaveStarted += HandleNewWave;
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null) return;

        EventManager.Instance.OnWaveStarted -= HandleNewWave;
    }

    private void HandleNewWave(int waveNumber)
    {
        levelText.text = waveNumber.ToString();
    }
}