using UnityEngine;

public class Catapult : Tower
{
    [SerializeField] private Transform verticalPivot;

    protected override void RotateVertical(float distance, float height)
    {
        float baseAngle = Mathf.Atan2(height, distance) * Mathf.Rad2Deg;
        float angle = Mathf.Clamp(baseAngle + distance * 2f, 10f, 75f);
        verticalPivot.localRotation = Quaternion.Euler(-angle, 0f, 0f);
    }
}