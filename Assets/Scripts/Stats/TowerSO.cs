using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerConfig", menuName = "Stats/TowerConfig")]
public class TowerSO : ScriptableObject
{
    [Header("Tower Info")]
    public string towerName;
    public Sprite towerImage;

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
    public List<TowerModeType> shootingModes = System.Enum
    .GetValues(typeof(TowerModeType))
    .Cast<TowerModeType>()
    .ToList();
    public TowerModeType startShootingMode = TowerModeType.First;

    [Header("Upgrades")]
    public UpgradeSO upgrade0;
    public UpgradeSO upgrade1;
    public UpgradeSO upgrade2;

    public List<UpgradeSO> GetUpgrades()
    {
        return new List<UpgradeSO>
        {
            upgrade0,
            upgrade1,
            upgrade2
        };
    }
}