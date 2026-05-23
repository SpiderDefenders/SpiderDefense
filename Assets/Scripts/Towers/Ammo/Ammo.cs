using UnityEngine;

public abstract class Ammo : MonoBehaviour
{
    protected GameObject target;
    [SerializeField] protected AmmoSO ammoConfig;
    protected bool shooted = false;
    private float damageMultiplier = 1f;

    public virtual void SetTarget(GameObject target)
    {
        shooted = true;
        this.target = target;
    }

    protected void HitTarget()
    {
        target.GetComponent<Enemy>().GetDamage(ammoConfig.damage * damageMultiplier);
        Destroy(gameObject);
        return;
    }

    public void SetDamageMultiplier(float damageMultiplier)
    {
        this.damageMultiplier = damageMultiplier;
    }
}