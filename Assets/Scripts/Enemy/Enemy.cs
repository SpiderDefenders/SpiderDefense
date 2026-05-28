using UnityEngine;
using UnityEngine.Splines;
using System;

public class Enemy : MonoBehaviour
{
    private SplineContainer splineContainer;
    [SerializeField] private EnemySO stats;
    [SerializeField] private Transform enemyRoot;

    private float t;
    private bool isDead = false;
    private float health;

    public Guid id;

    private void Start()
    {
        health = stats.maxHealth;
        id = System.Guid.NewGuid();
    }

    void Update()
    {
        if (isDead || splineContainer == null || splineContainer.Spline.Count < 2)
            return;
        
        t += (stats.movementSpeed / splineContainer.CalculateLength()) * Time.deltaTime;

        if (t >= 1f)
        {
            DealDamageAndDie();
            return;
        }
        t = Mathf.Clamp01(t);

        Vector3 splinePos = splineContainer.EvaluatePosition(t);
        float y = GetComponentInChildren<MoveAnt>().GetYPos(splinePos.x, splinePos.z);
        transform.position = new Vector3(splinePos.x, y, splinePos.z);
        transform.forward = splineContainer.EvaluateTangent(t);
    }
    
    public void SetSpline(SplineContainer spline)
    {
        splineContainer = spline;
        t = 0f;
    }

    private void DealDamageAndDie()
    {
        isDead = true;
        EventManager.Instance.EnemyReachedTheEnd(stats.attackDamage, this.gameObject);
        DestroyEnemy();
    }

    public void GetDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        if (health > 0) return;
        isDead = true;
        EventManager.Instance.EnemyDead(stats.moneyAfterDeath, this.gameObject);
        DestroyEnemy();
    }

    private void DestroyEnemy()
    {
        if (enemyRoot != null) Destroy(enemyRoot.gameObject);
        Destroy(gameObject);
    }

    public float GetProgress() { return t; }
    public float GetHealth() { return health; }
    public bool IsDead() { return isDead; }
}