using Unity.VisualScripting;
using UnityEngine;

public class Turret : Tower
{
    [SerializeField] private Transform verticalPivot;

    protected override void RotateVertical(float distance, float height)
    {

        float baseAngle = Mathf.Atan2(height, distance) * Mathf.Rad2Deg;
        float angle = Mathf.Clamp(baseAngle + distance * 2f, 0f, 75f);

        Quaternion verticalTargetRotation = Quaternion.Euler(angle, 0f, 0f);
        float currentAngle = verticalPivot.localEulerAngles.x;
        if (currentAngle > 180f) currentAngle -= 360f;
        float error = Mathf.Abs(Mathf.DeltaAngle(currentAngle, angle));
        if (error > aimTolerance)
        {
            isRotatedOnTarget = false;
        }

        verticalPivot.localRotation = Quaternion.Slerp(
            verticalPivot.localRotation,
            verticalTargetRotation,
            Time.deltaTime * 10f
        );
    }
}