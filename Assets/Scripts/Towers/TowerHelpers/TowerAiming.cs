using UnityEngine;

public class TowerAiming
{
    private Transform horizontalPivot;
    private Transform verticalPivot;
    private float aimTolerance;

    public bool IsAimed { get; private set; }

    public TowerAiming(Transform horizontalPivot, Transform verticalPivot, float aimTolerance)
    {
        this.horizontalPivot = horizontalPivot;
        this.verticalPivot = verticalPivot;
        this.aimTolerance = aimTolerance;
    }

    public void AimTarget(Transform target)
    {
        Vector3 direction = target.transform.position - horizontalPivot.position;
        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
        float distance = flatDirection.magnitude;
        float heightDifference = direction.y;

        IsAimed = true;
        RotateHorizontal(flatDirection);
        RotateVertical(distance, heightDifference);
    }

    private void RotateHorizontal(Vector3 flatDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(flatDirection);

        if (Quaternion.Angle(horizontalPivot.rotation, targetRotation) > aimTolerance)
        {
            IsAimed = false;
        }

        horizontalPivot.rotation = Quaternion.Slerp(
            horizontalPivot.rotation,
            targetRotation,
            Time.deltaTime * 15f
            );
    }
    protected virtual void RotateVertical(float distance, float height)
    {

        float baseAngle = Mathf.Atan2(height, distance) * Mathf.Rad2Deg;
        float angle = Mathf.Clamp(baseAngle + distance * 2f, 0f, 75f);

        Quaternion verticalTargetRotation = Quaternion.Euler(angle, 0f, 0f);
        float currentAngle = verticalPivot.localEulerAngles.x;
        if (currentAngle > 180f) currentAngle -= 360f;
        float error = Mathf.Abs(Mathf.DeltaAngle(currentAngle, angle));
        if (error > aimTolerance)
        {
            IsAimed = false;
        }

        verticalPivot.localRotation = Quaternion.Slerp(
            verticalPivot.localRotation,
            verticalTargetRotation,
            Time.deltaTime * 10f
        );
    }
}