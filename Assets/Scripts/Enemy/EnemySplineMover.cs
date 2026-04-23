using System;
using UnityEngine;
using UnityEngine.Splines;

public class EnemySplineMover : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private EnemySO stats;

    private float t;
    private bool isDead = false;
    private float health;
    
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        health = stats.maxHealth;
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

        transform.position = splineContainer.EvaluatePosition(t);
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
        EventManager.Instance.EnemyReachedTheEnd(stats.attackDamage);
        Destroy(gameObject);
    }

    public void GetDamage(float damage)
    {
        health -= damage;
        if (health > 0) return;
        isDead = true;
        // TODO
        // addMoney(stats.moneyAfterDeath);
        Destroy(gameObject);
    }

    public float GetProgress() { return t; }
    public bool IsDead() { return isDead; }
}