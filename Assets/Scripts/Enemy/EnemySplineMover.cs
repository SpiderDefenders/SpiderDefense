using System;
using UnityEngine;
using UnityEngine.Splines;

public class EnemySplineMover : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float speed = 2f;
    [SerializeField] private EnemySO stats;

    private float t;
    private bool isDead = false;
    
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if (isDead || splineContainer == null || splineContainer.Spline.Count < 2)
            return;
        
        t += (speed / splineContainer.CalculateLength()) * Time.deltaTime;

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

    // TODO temporary solution
    public void GetDamage(float damage)
    {
        isDead = true;
        Destroy(gameObject);
    }

    public float GetProgress() { return t; }
    public bool IsDead() { return isDead; }
}