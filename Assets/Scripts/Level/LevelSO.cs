using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Level")]
public class LevelSO : ScriptableObject
{
    [SerializeReference]
    public List<WaveSO> waves;
}