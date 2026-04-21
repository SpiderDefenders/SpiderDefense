using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Events/Wait")]
public class WaitEventSO : SpawnEventSO
{
    public float duration;

    public override IEnumerator Execute(LevelManager manager)
    {
        yield return new WaitForSeconds(duration);
    }
}