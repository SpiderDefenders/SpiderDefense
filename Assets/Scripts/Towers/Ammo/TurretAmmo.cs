using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TurretAmmo : Ammo
{
    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.transform.position - transform.position;
        float frameDist = speed * Time.deltaTime;

        if (dir.magnitude <= frameDist)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * frameDist, Space.World);
    }

}