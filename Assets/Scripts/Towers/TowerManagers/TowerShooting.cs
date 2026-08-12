using UnityEngine;

[System.Serializable]
public class TowerShooting
{
    protected Transform ammoSpawnPoint;
    protected GameObject ammoObject;
    private AudioSource audioSource;
    private GameObject ammoPrefab;

    protected bool isShooting = false;
    private float countdown = 0f;
    private float cooldown;

    public float DamageMultiplier { get; set; } = 1f;
    public float CooldownMultiplier { get; set; } = 1f;

    public TowerShooting(Transform ammoSpawnPoint, GameObject ammoPrefab,
        AudioSource audioSource, float cooldown)
    {
        this.ammoSpawnPoint = ammoSpawnPoint;
        this.ammoPrefab = ammoPrefab;
        this.audioSource = audioSource;
        this.cooldown = cooldown;

        ammoObject = Object.Instantiate(ammoPrefab, ammoSpawnPoint.position, ammoSpawnPoint.rotation, ammoSpawnPoint);
    }

    public void UpdateShooting(bool isAimed, GameObject target)
    {
        if (countdown <= 0f && isAimed && !isShooting)
        {
            isShooting = true;
            Shoot(target);
        }
        countdown -= Time.deltaTime;
    }

    public void UpdateAmmo()
    {
        float currCooldown = cooldown * CooldownMultiplier;
        if (countdown <= currCooldown / 4 && ammoObject == null)
        {
            ammoObject = Object.Instantiate(ammoPrefab, ammoSpawnPoint.position, ammoSpawnPoint.rotation, ammoSpawnPoint);
        }
    }

    protected virtual void Shoot(GameObject target)
    {
        countdown = cooldown * CooldownMultiplier;

        Ammo ammo = ammoObject.GetComponent<Ammo>();
        ammo.SetTarget(target);
        ammo.SetDamageMultiplier(DamageMultiplier);
        isShooting = false;
        ammoObject = null;
        AudioManager.Instance.PlaySFX(SoundID.TurretShot, audioSource);
    }

}