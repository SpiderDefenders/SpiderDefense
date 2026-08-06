using UnityEngine;

public abstract class Ammo : MonoBehaviour
{
    protected GameObject target;
    [SerializeField] protected AmmoSO ammoConfig;
    protected bool shooted = false;
    private float damageMultiplier = 1f;

    // shortest line to target
    protected virtual void Update()
    {
        if (!shooted) return;
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.transform.position - transform.position;
        float frameDist = ammoConfig.speed * Time.deltaTime;

        if (dir.magnitude <= frameDist)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * frameDist, Space.World);
    }

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