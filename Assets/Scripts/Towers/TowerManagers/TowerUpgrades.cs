using System.Collections.Generic;

// upgrades -> TODO jakiś refactor jakbyśmy to chcieli rozwijać
public class TowerUpgrades
{
    private TowerRange range;
    private TowerShooting shooting;
    private List<UpgradeSO> towerUpgrades;

    
    private bool[] purchasedUpgrades = new bool[] { false, false, false };

    public TowerUpgrades(TowerRange range, TowerShooting shooting, List<UpgradeSO> towerUpgrades) 
    {
        this.range = range;
        this.shooting = shooting;
        this.towerUpgrades = towerUpgrades;
    }

    public bool IsPurchased(int idx)
    {
        return purchasedUpgrades[idx];
    }

    public int Upgrade(int idx)
    {
        purchasedUpgrades[idx] = true;
        var upgrade = towerUpgrades[idx];

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

        return upgrade.cost;
    }

    private void UpgradeShootingCooldown(UpgradeSO upgrade)
    {
        shooting.CooldownMultiplier *= upgrade.factor;
    }

    private void UpgradeRange(UpgradeSO upgrade)
    {
        range.UpgradeRange(upgrade.factor);
    }

    private void UpgradeShootingDamage(UpgradeSO upgrade)
    {
        shooting.DamageMultiplier *= upgrade.factor;
    }

}