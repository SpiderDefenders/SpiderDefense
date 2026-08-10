using System.Collections.Generic;
using UnityEngine;


// rozbijamy na shooting, upgrades, range, targeting (z obrotami?)
public abstract class Tower : AdditionalPathBlocker, IPlacable, IDefense
{
    private ITile tile;

    [SerializeField] protected TowerSO towerConfig;
    [SerializeField] protected Transform ammoSpawnPoint;

    [Header("Pivoting")]
    [SerializeField] protected Transform horizontalPivot;
    [SerializeField] private Transform verticalPivot;
    [SerializeField] protected float aimTolerance = 10f;

    protected GameObject ammoObject;

    private GameObject rangeObject;
    protected GameObject target;
    private float yOffset = 0.05f;
    private float shootingCountdown = 0f;
    protected bool isRotatedOnTarget;
    private TowerModeManager modeManager;
    public PlacableType Type => PlacableType.Defense;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    private bool isPlaced = false;
    private bool isGameOver = false;
    private bool isShooting = false;

    private int value;

    //private TowerTargeting targeting;
    //private TowerShooting shooting;
    //private TowerUpgrades upgrades;
    //private TowerRange range;
    //private TowerAiming aiming;

    // upgrades -> TODO jakiś refactor
    private bool[] purchasedUpgrades = new bool[] { false, false, false };
    private float shootingDamageMultiplier = 1f;
    private float shootingCooldownMultiplier = 1f;

    //public Tower()
    //{
    //    range = new TowerRange();
    //    targeting = new TowerTargeting(range);
    //    shooting = new TowerShooting();
    //    upgrades = new TowerUpgrades();
    //    aiming = new TowerAiming();
    //    //upgrades = new TowerUpgrades(range, shooting);
    //}

    private new void OnEnable()
    {
        base.OnEnable();
        EventManager.Instance.OnGameOver += HandleGameOver;
    }

    private new void OnDisable()
    {
        base.OnDisable();
        EventManager.Instance.OnGameOver -= HandleGameOver;
    }

    private void HandleGameOver()
    {
        isGameOver = true;
    }

    private void Awake()
    {
        modeManager = new TowerModeManager(towerConfig.shootingModes, towerConfig.startShootingMode);
        CreateRangeObject();
        ammoObject = Instantiate(towerConfig.ammoPrefab, ammoSpawnPoint.position, ammoSpawnPoint.rotation, transform);
    }

    private void Update()
    {
        if (isGameOver)
            return;
        SetTarget();

        if (target != null)
        {
            isRotatedOnTarget = true;
            FollowTarget();
            UpdateShooting();
        }

        UpdateAmmo();
        shootingCountdown -= Time.deltaTime;
    }

    private void UpdateShooting()
    {
        if (shootingCountdown <= 0f && isRotatedOnTarget && !isShooting)
        {
            isShooting = true;
            Shoot();
        }
    }

    private void UpdateAmmo()
    {
        float currShootingCooldown = towerConfig.shootingCooldown * shootingCooldownMultiplier;
        if (shootingCountdown <= currShootingCooldown / 4 && ammoObject == null)
        {
            ammoObject = Instantiate(towerConfig.ammoPrefab, ammoSpawnPoint.position, ammoSpawnPoint.rotation, ammoSpawnPoint);
        }
    }

    private void CreateRangeObject()
    {
        rangeObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rangeObject.tag = "Range";
        rangeObject.name = "Range";
        rangeObject.transform.SetParent(transform);
        rangeObject.transform.localPosition = new Vector3(0f, yOffset, 0f);
        rangeObject.transform.localRotation = Quaternion.identity;
        rangeObject.transform.localScale = new Vector3(towerConfig.rangeRadius * 2f, 0.01f, towerConfig.rangeRadius * 2f);

        rangeObject.GetComponent<MeshRenderer>().material = towerConfig.rangeMaterial;
    }

    private void CreateCollider()
    {
        SphereCollider col = gameObject.AddComponent<SphereCollider>();
        col.radius = towerConfig.rangeRadius;
        col.center = new Vector3(0f, yOffset, 0f);
        col.isTrigger = true;
    }

    private void RefreshEnemiesInRange()
    {
        enemiesInRange.Clear();

        SphereCollider col = GetComponent<SphereCollider>();
        Vector3 worldCenter = transform.position + col.center;

        Collider[] hits = Physics.OverlapSphere(worldCenter, col.radius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                enemiesInRange.Add(hit.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    private void SetTarget()
    {
        GameObject bestTarget = null;
        float bestValue = -Mathf.Infinity;

        // go backwards to avoid errors when removing enemies
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            GameObject enemy = enemiesInRange[i];

            if (enemy == null || enemy.GetComponent<Enemy>().IsDead())
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }

            float value = modeManager.GetModeValue(enemy.GetComponent<Enemy>());

            if (value > bestValue)
            {
                bestValue = value;
                bestTarget = enemy;
            }
        }

        target = bestTarget;
    }

    protected virtual void Shoot()
    {
        float currShootingCooldown = towerConfig.shootingCooldown * shootingCooldownMultiplier;
        shootingCountdown = currShootingCooldown;

        Ammo ammo = ammoObject.GetComponent<Ammo>();
        ammo.SetTarget(target);
        ammo.SetDamageMultiplier(shootingDamageMultiplier);
        isShooting = false;
        ammoObject = null;
        AudioManager.Instance.PlaySFX(SoundID.TurretShot, GetComponent<AudioSource>());
    }

    public void OnPlaced(ITile tile)
    {
        this.tile = tile;
        rangeObject.SetActive(false);
        CreateCollider();
        RefreshEnemiesInRange();
        isPlaced = true;

        value = towerConfig.cost;
        CurrencyManager.Instance.RemoveMoney(towerConfig.cost);
    }

    public bool IsPlaced => isPlaced;
    public int Cost => towerConfig.cost;
    public string Name => towerConfig.towerName;
    public Sprite Image => towerConfig.towerImage;
    public TowerModeManager TowerModeManager => modeManager;
    public int Value => value;

    public void OnClick()
    {
        if (isBlocked) return;
        rangeObject.SetActive(true);
    }

    public void OnUnClick()
    {
        rangeObject.SetActive(false);
    }

    public void OnRemoved()
    {
        tile.Remove();
        CurrencyManager.Instance.OnTowerRemoved(value);
        Destroy(gameObject);
    }

    public Vector3 GetPlacingOffset()
    {
        return towerConfig.placingOffset;
    }

    public virtual void FollowTarget()
    {
        Vector3 direction = target.transform.position - horizontalPivot.position;
        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
        float distance = flatDirection.magnitude;
        float heightDifference = direction.y;

        RotateHorizontal(flatDirection);
        RotateVertical(distance, heightDifference);
    }

    private void RotateHorizontal(Vector3 flatDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
        
        if (Quaternion.Angle(horizontalPivot.rotation, targetRotation) > aimTolerance)
        {
            isRotatedOnTarget = false;
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
            isRotatedOnTarget = false;
        }

        verticalPivot.localRotation = Quaternion.Slerp(
            verticalPivot.localRotation,
            verticalTargetRotation,
            Time.deltaTime * 10f
        );
    }

    // upgrades
    public List<UpgradeSO> GetUpgrades()
    {
        return towerConfig.GetUpgrades();
    }

    public bool IsUpgradePurchased(int idx)
    {
        return purchasedUpgrades[idx];
    }

    public void Upgrade(int idx)
    {
        purchasedUpgrades[idx] = true;
        var upgrade = towerConfig.GetUpgrades()[idx];
        value += upgrade.cost;

        switch (upgrade.type)
        {
            case UpgradeType.ShootingCooldown:
                UpgradeShootingCooldown(upgrade);
                break;

            case UpgradeType.Range:
                UpgradeRange(upgrade);
                break;

            case UpgradeType.ShootingDamage:
                UpgradeShootingDamage(upgrade);
                break;
        }
    }

    private void UpgradeShootingCooldown(UpgradeSO upgrade)
    {
        shootingCooldownMultiplier *= upgrade.factor;
    }

    private void UpgradeRange(UpgradeSO upgrade)
    {
        float r = towerConfig.rangeRadius * upgrade.factor;

        rangeObject.transform.localScale = new Vector3(r * 2f, 0.01f, r * 2f);
        SphereCollider col = GetComponent<SphereCollider>();
        col.radius = r;

        RefreshEnemiesInRange();
    }

    private void UpgradeShootingDamage(UpgradeSO upgrade)
    {
        shootingDamageMultiplier *= upgrade.factor;
    }
}