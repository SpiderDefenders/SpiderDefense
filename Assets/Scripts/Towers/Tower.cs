using System;
using UnityEngine;

public abstract class Tower : MonoBehaviour, IPlacable, IDefense
{
    private ITile tile;
    
    [Header("Placing")]
    public Vector3 placingOffset;

    [Header("Targeting")]
    public GameObject target;
    
    [SerializeField] private Transform horizontalPivot;

    public PlacableType Type => PlacableType.Defense;

    private void Update()
    {
        if (target == null) return;
        FollowTarget();
    }

    public void OnPlaced(ITile tile)
    {
        this.tile = tile;
    }

    public void OnRemoved()
    {
        Destroy(gameObject);
    }

    public Vector3 GetPlacingOffset()
    {
        return placingOffset;
    }

    public virtual void FollowTarget()
    {
        if (target == null) return;

        Vector3 direction = target.transform.position - horizontalPivot.position;

        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);

        if (flatDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
            horizontalPivot.rotation = targetRotation;
        }

        float distance = flatDirection.magnitude;
        float heightDifference = direction.y;

        CalculateAndModifyLaunchAngle(distance, heightDifference);
    }
    
    protected virtual void CalculateAndModifyLaunchAngle(float distance, float height)
    {
        return;
    }
}