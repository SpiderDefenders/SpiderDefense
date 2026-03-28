using UnityEngine;
using UnityEngine.Splines;

public class EnemySplineMover : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float speed = 2f;

    private float t;
    private bool isDead = false;

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
        EventManager.Instance.EnemyReachedTheEnd();
        Destroy(gameObject);
    }
}