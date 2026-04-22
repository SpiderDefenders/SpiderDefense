using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour, IPlacable, IDefense
{
    private ITile tile;
    
    [Header("Placing")]
    public Vector3 placingOffset;

    [Header("Range")]
    [SerializeField] private float rangeRadius = 1.5f;
    [SerializeField] private Material rangeMaterial;
    private GameObject rangeObject;
    private float yOffset = 0.05f;

    [Header("Shooting")]
    [SerializeField] private float shootingCooldown = 1f; 
    private float shootingCountdown = 0f;
    [SerializeField] private GameObject ammoPrefab;
    [SerializeField] private Transform ammoSpawnPoint;

    [Header("Targeting")]
    public GameObject target;
    
    [SerializeField] private Transform horizontalPivot;

    public PlacableType Type => PlacableType.Defense;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    private bool isPlaced = false;


    private void Awake()
    {
        CreateRangeObject();
    }
    private void Update()
    {
        SetTarget();

        if (target != null)
        {
            FollowTarget();

            if (shootingCountdown <= 0f)
            {
                Shoot();
                shootingCountdown = shootingCooldown;
            }
        }

        shootingCountdown -= Time.deltaTime;
    }

    private void CreateRangeObject()
    {
        rangeObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rangeObject.tag = "Range";
        rangeObject.name = "Range";
        rangeObject.transform.SetParent(transform);
        rangeObject.transform.localPosition = new Vector3(0f, yOffset, 0f);
        rangeObject.transform.localRotation = Quaternion.identity;
        rangeObject.transform.localScale = new Vector3(rangeRadius * 2f, 0.01f, rangeRadius * 2f);

        rangeObject.GetComponent<MeshRenderer>().material = rangeMaterial;
    }

    private void CreateCollider()
    {
        SphereCollider col = gameObject.AddComponent<SphereCollider>();
        col.radius = rangeRadius;
        col.center = new Vector3(0f, yOffset, 0f);
        col.isTrigger = true;
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
        float maxProgress = -Mathf.Infinity;

        // go backwards to avoid errors when removing enemies
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            GameObject enemy = enemiesInRange[i];

            if (enemy == null || enemy.GetComponent<EnemySplineMover>().IsDead())
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }

            float progress = enemy.GetComponent<EnemySplineMover>().GetProgress();

            if (progress > maxProgress)
            {
                maxProgress = progress;
                bestTarget = enemy;
            }
        }

        target = bestTarget;
    }

    private void Shoot()
    {
        GameObject ammoObject = Instantiate(ammoPrefab, ammoSpawnPoint.position, ammoSpawnPoint.rotation);
        Ammo ammo = ammoObject.GetComponent<Ammo>();
        ammo.SetTarget(target);

    }

    public void OnPlaced(ITile tile)
    {
        this.tile = tile;
        rangeObject.SetActive(false);
        CreateCollider();
        isPlaced = true;
    }

    public bool IsPlaced() {  return isPlaced; }

    public void OnClick()
    {
        rangeObject.SetActive(true);
    }

    public void OnUnClick()
    {
        rangeObject.SetActive(false);
    }

    public void OnRemoved()
    {
        Destroy(gameObject);
    }

    public Vector3 GetPlacingOffset()
    {
        return placingOffset;
    }

    public virtual void FollowTarget()
    {
        Vector3 direction = target.transform.position - horizontalPivot.position;

        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);

        if (flatDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
            horizontalPivot.rotation = Quaternion.Slerp(
                horizontalPivot.rotation,
                targetRotation,
                Time.deltaTime * 8f
                );
        }

        float distance = flatDirection.magnitude;
        float heightDifference = direction.y;

        CalculateAndModifyLaunchAngle(distance, heightDifference);
    }
    
    protected virtual void CalculateAndModifyLaunchAngle(float distance, float height)
    {
        return;
    }
}