using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveAnt : MonoBehaviour
{
    [Header("Attack Settings")] [SerializeField]
    private float attackDistance = 0.5f;

    [SerializeField] private float attackDuration = 0.5f;

    [Header("References")] [SerializeField]
    private List<LegTarget> targets;

    [SerializeField] private List<Transform> lastJoints;
    [SerializeField] private float bodyHeight = 0.5f;

    private Rigidbody rb;

    private Vector3 inputDir;
    private GameObject enemy;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public float GetYPos(float x, float z)
    {
        return CalculateBodyHeight();
    }

    private float CalculateBodyHeight()
    {
        if (targets.Count == 0) return rb.position.y;
        float result = 0;
        foreach (LegTarget t in targets)
            result += t.transform.position.y;
        return (result / targets.Count) + bodyHeight;
    }
}