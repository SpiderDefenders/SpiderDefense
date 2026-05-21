using UnityEngine;

public abstract class Ammo : MonoBehaviour
{
    protected GameObject target;
    [SerializeField] protected AmmoSO ammoConfig;
    protected bool shooted = false;

    public virtual void SetTarget(GameObject target)
    {
        shooted = true;
        this.target = target;
    }

    protected void HitTarget()
    {
        target.GetComponent<Enemy>().GetDamage(ammoConfig.damage);
        Destroy(gameObject);
        return;
    }
}