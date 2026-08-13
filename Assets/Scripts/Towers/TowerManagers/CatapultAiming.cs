using UnityEngine;

public class CatapultAiming : TowerAiming
{
    public CatapultAiming(Transform horizontalPivot, Transform verticalPivot, float aimTolerance) : base(horizontalPivot, verticalPivot, aimTolerance)
    {}
    protected override void RotateVertical(float distance, float height)
    {
        return;
    }
}