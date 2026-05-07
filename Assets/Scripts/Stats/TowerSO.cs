using UnityEngine;

[CreateAssetMenu(fileName = "TowerConfig", menuName = "Stats/TowerConfig")]
public class TowerSO : ScriptableObject
{
    [Header("Placing")]
    public Vector3 placingOffset;

    [Header("Cost")]
    public int cost = 100;

    [Header("Range")]
    public float rangeRadius = 1.5f;
    public Material rangeMaterial;
    
    [Header("Shooting")]
    public float shootingCooldown = 1f;
    public GameObject ammoPrefab;
}