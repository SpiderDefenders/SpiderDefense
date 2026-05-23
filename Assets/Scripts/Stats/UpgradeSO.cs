using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Stats/UpgradeConfig")]
public class UpgradeSO : ScriptableObject
{
    public Sprite upgardeImage;
    public int cost;
    public float factor;
    public UpgradeType type;
}