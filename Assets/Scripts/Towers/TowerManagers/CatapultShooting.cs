using UnityEngine;

public class CatapultShooting : TowerShooting
{
    private Transform rotatingPivot;
    private Transform catapultArm;
    private float gravity = 5f;
    private float angleGoal = 90;
    private float backTime = 0.4f;
    private float armBackDelay = 0.2f;
    private GameObject tower;

    public CatapultShooting(Transform ammoSpawnPoint, GameObject ammoPrefab, AudioSource audioSource, float cooldown, Transform rotatingPivot, Transform catapultArm, GameObject tower) 
        : base(ammoSpawnPoint, ammoPrefab, audioSource, cooldown)
    {
        this.rotatingPivot = rotatingPivot;
        this.catapultArm = catapultArm;
        this.tower = tower;

        catapultArm.rotation = rotatingPivot.rotation;
    }

    private float CalculateLinearSpeed(Vector3 shootingPos, GameObject target)
    {
        float dist = Mathf.Sqrt(
            Mathf.Pow(target.transform.position.x - shootingPos.x, 2)
            + Mathf.Pow(target.transform.position.z - shootingPos.z, 2));

        float distY = Mathf.Abs(target.transform.position.y - shootingPos.y);
        return dist * Mathf.Sqrt(gravity * 0.5f / distY);
    }
    protected override void Shoot(GameObject target)
    {
        Vector3 pivot = rotatingPivot.position;
        Vector3 shootingPos = Quaternion.AngleAxis(angleGoal, Vector3.right) * (ammoSpawnPoint.position - pivot) + pivot;
        float angleSpeed = CalculateLinearSpeed(shootingPos, target) / (pivot - ammoSpawnPoint.position).magnitude;
        float time = angleGoal / (angleSpeed * 180 / 3.14f);

        float previousAngle = 0f;

        LeanTween.value(tower, 0f, angleGoal, time)
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

                base.Shoot(target);
                CatapultArmBack();
            });
    }

    private void CatapultArmBack()
    {
        float previousAngle = angleGoal;
        LeanTween.value(tower, angleGoal, 0f, backTime)
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