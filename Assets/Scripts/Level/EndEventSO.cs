using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Events/End")]
public class EndEventSO : SpawnEventSO
{
    public override IEnumerator Execute(LevelManager manager)
    {
        EventManager.Instance.WaveCompleted(manager.GetCurrentWave());
        yield break;
    }
}