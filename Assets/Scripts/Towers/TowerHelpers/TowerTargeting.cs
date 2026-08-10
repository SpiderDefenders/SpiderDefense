using System.Collections.Generic;
using UnityEngine;

public class TowerTargeting
{
    private GameObject target;
    private TowerRange range;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    public TowerModeManager ModeManager { get; private set; }
    public TowerTargeting(TowerRange range, List<TowerModeType> shootingModes, TowerModeType startMode) { 
        this.range = range;
        ModeManager = new TowerModeManager(shootingModes, startMode);
    }

    public void AddEnemy(GameObject enemy)
    {   
        enemiesInRange.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemiesInRange.Remove(enemy);
    }

    public void ClearEnemies()
    {
        enemiesInRange.Clear();
    }

    public GameObject FindTarget()
    {
        GameObject bestTarget = null;
        float bestValue = -Mathf.Infinity;

        // go backwards to avoid errors when removing enemies
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            GameObject enemy = enemiesInRange[i];

            if (enemy == null || enemy.GetComponent<Enemy>().IsDead())
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }

            float value = ModeManager.GetModeValue(enemy.GetComponent<Enemy>());

            if (value > bestValue)
            {
                bestValue = value;
                bestTarget = enemy;
            }
        }

        target = bestTarget;
        return target;
    }
}