using UnityEngine;


public abstract class Ammo : MonoBehaviour
{
    protected GameObject target;
    [SerializeField] protected float speed;
    [SerializeField] protected float damage=1f;

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }


    protected void HitTarget()
    {
        Debug.Log("hit");
        target.GetComponent<EnemySplineMover>().GetDamage(damage);
        Destroy(gameObject);
        return;
    }


}