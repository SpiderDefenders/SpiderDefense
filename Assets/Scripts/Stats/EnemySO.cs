using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Stats/EnemyStats")]
public class EnemySO : ScriptableObject
{
    public float movementSpeed = 5f;
    public int maxHealth = 10;
    public float attackDamage = 10f;
    public float moneyAfterDeath = 10.0f;
}