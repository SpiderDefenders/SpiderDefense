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


    private GameObject target;
    public PlacableType Type => PlacableType.Defense;
    private bool isPlaced = false;
    private bool isGameOver = false;

    private int value;

    private TowerTargeting targeting;
    protected TowerShooting shooting;
    private TowerUpgrades upgrades;
    private TowerRange range;
    protected TowerAiming aiming;

    // upgrades -> TODO jakiś refactor
    private bool[] purchasedUpgrades = new bool[] { false, false, false };

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
        CreateManagers();
        range.CreateRangeObject(towerConfig.rangeRadius, towerConfig.rangeMaterial, transform);
    }

    protected virtual void CreateManagers()
    {
        range = gameObject.AddComponent<TowerRange>();
        targeting = new TowerTargeting(range, towerConfig.shootingModes, towerConfig.startShootingMode);
        shooting = new TowerShooting(ammoSpawnPoint, towerConfig.ammoPrefab, GetComponent<AudioSource>(), towerConfig.shootingCooldown);
        upgrades = new TowerUpgrades();
        aiming = new TowerAiming(horizontalPivot, verticalPivot, aimTolerance);
    }

    private void Update()
    {
        if (isGameOver)
            return;
        target = targeting.FindTarget();

        if (target != null)
        {
            aiming.AimTarget(target.transform);
            shooting.UpdateShooting(aiming.IsAimed, target);
        }

        shooting.UpdateAmmo();
    }

    public void OnPlaced(ITile tile)
    {
        this.tile = tile;
        range.SetActive(false);
        range.CreateCollider(towerConfig.rangeRadius);
        range.RefreshEnemiesInRange();
        isPlaced = true;

        value = towerConfig.cost;
        CurrencyManager.Instance.RemoveMoney(towerConfig.cost);
    }

    public bool IsPlaced => isPlaced;
    public int Cost => towerConfig.cost;
    public string Name => towerConfig.towerName;
    public Sprite Image => towerConfig.towerImage;
    public TowerModeManager TowerModeManager => targeting.ModeManager;
    public int Value => value;

    public void OnClick()
    {
        if (isBlocked) return;
        range.SetActive(true);
    }

    public void OnUnClick()
    {
        range.SetActive(false);
    }

    public void OnRemoved()
    {
        tile.Remove();
        CurrencyManager.Instance.OnTowerRemoved(value);
        Destroy(gameObject);
    }

    // TODO -> simple getter
    public Vector3 GetPlacingOffset()
    {
        return towerConfig.placingOffset;
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
        shooting.CooldownMultiplier *= upgrade.factor;
    }

    private void UpgradeRange(UpgradeSO upgrade)
    {
        float r = towerConfig.rangeRadius * upgrade.factor;
        range.UpgradeRange(r);
    }

    private void UpgradeShootingDamage(UpgradeSO upgrade)
    {
        shooting.DamageMultiplier *= upgrade.factor;
    }
}