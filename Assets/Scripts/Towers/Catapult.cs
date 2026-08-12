using UnityEngine;

public class Catapult : Tower
{
    [Header("Amination")]
    [SerializeField] private Transform rotatingPivot;
    [SerializeField] private Transform catapultArm;

    private void Start()
    {
        catapultArm.rotation = rotatingPivot.rotation;
    }

    protected override void CreateManagers()
    {
        base.CreateManagers();
        aiming = new CatapultAiming(horizontalPivot, null, aimTolerance);
        shooting = new CatapultShooting(ammoSpawnPoint, towerConfig.ammoPrefab, GetComponent<AudioSource>(), 
            towerConfig.shootingCooldown, rotatingPivot, catapultArm, gameObject);
    }
}