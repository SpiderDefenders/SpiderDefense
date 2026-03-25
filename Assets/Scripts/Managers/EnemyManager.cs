using System;
using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using Unity.VisualScripting;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxEnemies = 10;

    private int spawnedCount = 0;

    private void Start()
    {
        EventManager.Instance.OnPathGenerated += StartSpawning;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnPathGenerated -= StartSpawning;
    }

    private void StartSpawning()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (spawnedCount < maxEnemies)
        {
            SpawnEnemy();
            spawnedCount++;

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        EnemySplineMover mover = enemy.GetComponent<EnemySplineMover>();

        if (mover != null)
        {
            mover.SetSpline(splineContainer);
        }
        else
        {
            Debug.LogWarning("Enemy prefab is missing EnemySplineMover!");
        }
    }
}