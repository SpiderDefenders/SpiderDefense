using UnityEngine;
using UnityEngine.Splines;

public class EnemySplineMover : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float speed = 2f;

    private float t; // 0 → 1 along spline

    void Update()
    {
        if (splineContainer == null || splineContainer.Spline.Count < 2)
            return;

        // Move along spline
        t += (speed / splineContainer.CalculateLength()) * Time.deltaTime;

        t = Mathf.Clamp01(t);

        transform.position = splineContainer.EvaluatePosition(t);
        transform.forward = splineContainer.EvaluateTangent(t);
    }
    
    public void SetSpline(SplineContainer spline)
    {
        splineContainer = spline;
        t = 0f; // reset progress
    }
}