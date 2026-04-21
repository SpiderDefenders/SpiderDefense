using System.Collections;
using UnityEngine;

public abstract class SpawnEventSO : ScriptableObject
{
    public abstract IEnumerator Execute(LevelManager manager);
}