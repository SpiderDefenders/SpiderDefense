using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Stats/EnemyStats")]
public class EnemySO : ScriptableObject
{
    public float movementSpeed = 5f;
    public int maxHealth = 10;
    public int attackDamage = 10;
    public int moneyAfterDeath = 10;
}