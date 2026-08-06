using UnityEngine;

public class CatapultAmmo : Ammo
{
    private float gravity;
    private float overallTime;
    private float timeElapsed = 0f;
    private Vector3 startPos;
    protected override void Update()
    {
        if (!shooted) return;
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        float dist = Mathf.Sqrt(
            Mathf.Pow(target.transform.position.x - transform.position.x, 2) 
            + Mathf.Pow(target.transform.position.z - transform.position.z, 2));
        timeElapsed += Time.deltaTime;
        float timeLeft = overallTime - timeElapsed;
        float speed = dist / timeLeft;

        if (timeLeft < Time.deltaTime)
        {
            HitTarget();
            return;
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        Vector3 currPos = transform.position;
        Vector3 pos = transform.position + dir * speed * Time.deltaTime;
        pos.y = startPos.y - gravity * timeElapsed * timeElapsed / 2;
        
        transform.position = pos;


    }
    public override void SetTarget(GameObject target)
    {
        base.SetTarget(target);
        float distY = Mathf.Abs(target.transform.position.y - transform.position.y);
        overallTime = Mathf.Sqrt(2 * distY / gravity);
        startPos = transform.position;
    }

    public void SetGravity(float gravity)
    {
        this.gravity = gravity;
    }
}