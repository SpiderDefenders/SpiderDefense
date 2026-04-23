using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;

    [Header("Enemies")]
    [SerializeField] private List<EnemyDefinition> enemies;

    private Dictionary<EnemyType, GameObject> enemyLookup;

    private void Awake()
    {
        enemyLookup = new Dictionary<EnemyType, GameObject>();

        foreach (var e in enemies)
        {
            if (!enemyLookup.ContainsKey(e.type))
                enemyLookup.Add(e.type, e.prefab);
            else
                Debug.LogWarning($"Duplicate enemy type: {e.type}");
        }
    }

    public void SpawnSingleEnemy()
    {
        SpawnEnemy(EnemyType.Basic);
    }

    public void SpawnEnemy(EnemyType type)
    {
        if (!enemyLookup.TryGetValue(type, out var prefab))
        {
            Debug.LogError($"No prefab for enemy type: {type}");
            return;
        }

        GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity, transform);

        var mover = enemy.GetComponent<EnemySplineMover>();

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