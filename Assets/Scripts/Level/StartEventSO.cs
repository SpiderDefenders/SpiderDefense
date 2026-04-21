using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Events/Start")]
public class StartEventSO : SpawnEventSO
{
    public override IEnumerator Execute(LevelManager manager)
    {
        EventManager.Instance.WaveStarted(manager.GetCurrentWave());
        yield break;
    }
}