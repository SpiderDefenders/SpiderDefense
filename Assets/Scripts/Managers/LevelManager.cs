using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Level")] [SerializeField] private LevelSO level;

    [Header("Settings")]
    [SerializeField] private float delayBetweenWaves = 3f;
    [SerializeField] private float delayAfterAdditionalPathPlaced = 5f;

    private Coroutine levelRoutine;
    private EnemyManager enemyManager;
    private int currentWave = 0;
    
    private bool isGameOver = false;
    private int currentNumberOfEnemies = 0;
    private bool canStartNextWave = false;
    
    private void OnEnable()
    {
        EventManager.Instance.OnPathGenerated += StartLevel;
        enemyManager = FindAnyObjectByType<EnemyManager>();
        EventManager.Instance.OnGameOver += HandleGameOver;
        EventManager.Instance.OnEnemyDead += EnemyDead;
        EventManager.Instance.OnEnemyReachedTheEnd += EnemyDead;
        EventManager.Instance.OnAdditionalPathPlacingCompleted += EnableStartingNextWave;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnPathGenerated -= StartLevel;
        EventManager.Instance.OnGameOver -= HandleGameOver;
        EventManager.Instance.OnEnemyDead -= EnemyDead;
        EventManager.Instance.OnEnemyReachedTheEnd -= EnemyDead;
        EventManager.Instance.OnAdditionalPathPlacingCompleted -= EnableStartingNextWave;
    }
    
    private void EnableStartingNextWave()
    {
        canStartNextWave = true;
    }

    private void StartLevel()
    {
        Time.timeScale = 1f;
        Debug.Log("Level Started");
        if (level == null)
        {
            Debug.LogError("LevelSO is not assigned!");
            return;
        }

        if (levelRoutine != null)
            StopCoroutine(levelRoutine);

        currentWave = 0;
        levelRoutine = StartCoroutine(RunLevel());
    }

    private IEnumerator RunLevel()
    {
        for (int i = 0; i < level.waves.Count; i++)
        {
            if (isGameOver) yield break;
            var wave = level.waves[i];
            currentWave = i + 1;

            Debug.Log($"--- Wave {i:00} START ---");

            yield return StartCoroutine(RunWave(wave));

            EventManager.Instance.WaveCompleted(currentWave, i==level.waves.Count-1);
            Debug.Log($"--- Wave {i:00} END ---");

            if (i < level.waves.Count - 1)
            {
                yield return new WaitForSeconds(delayBetweenWaves);
            }
            else
            {
                break;
            }

            while (!canStartNextWave)
            {
                yield return new WaitForSeconds(0.05f);
            }

            canStartNextWave = false;
            yield return new WaitForSeconds(delayAfterAdditionalPathPlaced);
        }
        
        EventManager.Instance.LevelCompleted();
        Debug.Log("Level Finished");
    }

    private IEnumerator RunWave(WaveSO wave)
    {
        foreach (var evt in wave.events)
        {
            if (isGameOver) yield break;
            yield return StartCoroutine(evt.Execute(this));
        }
    }

    public void SpawnEnemy(EnemyType type)
    {
        currentNumberOfEnemies += 1;
        enemyManager.SpawnEnemy(type);
    }
    
    private void HandleGameOver()
    {
        isGameOver = true;

        if (levelRoutine != null)
        {
            StopCoroutine(levelRoutine);
            levelRoutine = null;
        }

        Debug.Log("Level stopped due to Game Over");
    }
    
    private void EnemyDead(int moneyAfterDead)
    {
        currentNumberOfEnemies -= 1;
    }
    
    public int GetCurrentWave() => currentWave;
    public int GetCurrentNumberOfEnemies() => currentNumberOfEnemies;
    public float GetDelayAfterAdditionalPathPlaced() => delayAfterAdditionalPathPlaced;
}