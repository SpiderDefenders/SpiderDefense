using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Level")] [SerializeField] private LevelSO level;

    [Header("Settings")] [SerializeField] private float delayBetweenWaves = 3f;

    private Coroutine levelRoutine;
    private EnemyManager enemyManager;
    private int currentWave = 0;
    
    private void Start()
    {
        EventManager.Instance.OnPathGenerated += StartLevel;
        enemyManager = FindAnyObjectByType<EnemyManager>();
    }

    private void OnDisable()
    {
        EventManager.Instance.OnPathGenerated -= StartLevel;
    }

    private void StartLevel()
    {
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
        Debug.Log("Level Started");

        for (int i = 0; i < level.waves.Count; i++)
        {
            var wave = level.waves[i];
            currentWave = i + 1;

            Debug.Log($"--- Wave {i:00} START ---");

            yield return StartCoroutine(RunWave(wave));

            Debug.Log($"--- Wave {i:00} END ---");

            // wait between waves (except after last)
            if (i < level.waves.Count - 1)
            {
                yield return new WaitForSeconds(delayBetweenWaves);
            }
        }

        Debug.Log("Level Finished");
    }

    private IEnumerator RunWave(WaveSO wave)
    {
        foreach (var evt in wave.events)
        {
            yield return StartCoroutine(evt.Execute(this));
        }
    }

    public void SpawnEnemy(EnemyType type)
    {
        enemyManager.SpawnEnemy(type);
    }
    
    public int GetCurrentWave() => currentWave;
}