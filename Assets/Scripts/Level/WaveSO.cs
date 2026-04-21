using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Wave")]
public class WaveSO : ScriptableObject
{
    public List<SpawnEventSO> events;
}