using UnityEngine;

public abstract class Ammo : MonoBehaviour
{
    protected GameObject target;
    [SerializeField] protected AmmoSO ammoConfig;

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    protected void HitTarget()
    {
        target.GetComponent<Enemy>().GetDamage(ammoConfig.damage);
        Destroy(gameObject);
        return;
    }
}