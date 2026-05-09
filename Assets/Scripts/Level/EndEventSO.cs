using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Events/End")]
public class EndEventSO : SpawnEventSO
{
    public override IEnumerator Execute(LevelManager manager)
    {
        while (manager.GetCurrentNumberOfEnemies() > 0)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }
}