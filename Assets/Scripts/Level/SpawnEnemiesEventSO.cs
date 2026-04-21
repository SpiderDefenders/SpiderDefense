using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "TD/Events/SpawnEnemies")]
public class SpawnEnemiesEventSO : SpawnEventSO
{
    public EnemyType enemyType;
    public int count;
    public float delayBetween;

    public override IEnumerator Execute(LevelManager manager)
    {
        for (int i = 0; i < count; i++)
        {
            manager.SpawnEnemy(enemyType);
            yield return new WaitForSeconds(delayBetween);
        }
    }
}