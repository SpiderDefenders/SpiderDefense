using UnityEngine;

public class Catapult : Tower
{
    [SerializeField] private Transform rotatingPivot;
    [SerializeField] private Transform catapultArm;
    [SerializeField] private float gravity=5f;
    [Header("Animation")]
    [SerializeField] private float angleGoal = 90;
    [SerializeField] private float backTime = 0.4f;
    [SerializeField] private float armBackDelay = 0.2f;

    private void Start()
    {
        catapultArm.rotation = rotatingPivot.rotation;
    }

    protected override void RotateVertical(float distance, float height)
    {
        return;
    }

    private float CalculateLinearSpeed(Vector3 shootingPos)
    {
        float dist = Mathf.Sqrt(
            Mathf.Pow(target.transform.position.x - shootingPos.x, 2)
            + Mathf.Pow(target.transform.position.z - shootingPos.z, 2));

        float distY = Mathf.Abs(target.transform.position.y - shootingPos.y);
        return dist * Mathf.Sqrt(gravity * 0.5f / distY);
    }
    protected override void Shoot()
    {
        Vector3 pivot = rotatingPivot.position;
        Vector3 shootingPos = Quaternion.AngleAxis(angleGoal, Vector3.right) * (ammoSpawnPoint.position - pivot) + pivot;
        float angleSpeed = CalculateLinearSpeed(shootingPos) / (pivot - ammoSpawnPoint.position).magnitude;
        float time = angleGoal / (angleSpeed * 180 / 3.14f);

        float previousAngle = 0f;

        LeanTween.value(gameObject, 0f, angleGoal, time)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnUpdate((float val) =>
            {

                float delta = val - previousAngle;

                catapultArm.RotateAround(
                    rotatingPivot.position,
                    rotatingPivot.right,
                    delta
                );
                previousAngle = val;
            })
            .setOnComplete(() =>
            {
                CatapultAmmo ammo = ammoObject.GetComponent<CatapultAmmo>();
                ammo.SetGravity(gravity);

                base.Shoot();

                CatapultArmBack();
            });
    }

    private void CatapultArmBack()
    {
        float previousAngle = angleGoal;
        LeanTween.value(gameObject, angleGoal, 0f, backTime)
            .setDelay(armBackDelay)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnUpdate((float val) =>
            {

                float delta = val - previousAngle;

                catapultArm.RotateAround(
                    rotatingPivot.position,
                    rotatingPivot.right,
                    delta
                );
                previousAngle = val;
            });
    }
}