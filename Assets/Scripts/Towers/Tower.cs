using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : AdditionalPathBlocker, IPlacable, IDefense
{
    private ITile tile;

    [SerializeField] private TowerSO towerConfig;
    [SerializeField] Transform ammoSpawnPoint;

    [Header("Pivoting")]
    [SerializeField] private Transform horizontalPivot;
    //[SerializeField] private Transform verticalPivot; 
    
    private GameObject rangeObject;
    private GameObject target;
    private float yOffset = 0.05f;
    private float shootingCountdown = 0f;
    public PlacableType Type => PlacableType.Defense;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    private bool isPlaced = false;
    private bool isGameOver = false;

    private int value;

    private void OnEnable()
    {
        base.OnEnable();
        EventManager.Instance.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
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
        CreateRangeObject();
    }
    private void Update()
    {
        if (isGameOver)
            return;
        SetTarget();

        if (target != null)
        {
            FollowTarget();

            if (shootingCountdown <= 0f)
            {
                Shoot();
                shootingCountdown = towerConfig.shootingCooldown;
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
        rangeObject.transform.localScale = new Vector3(towerConfig.rangeRadius * 2f, 0.01f, towerConfig.rangeRadius * 2f);

        rangeObject.GetComponent<MeshRenderer>().material = towerConfig.rangeMaterial;
    }

    private void CreateCollider()
    {
        SphereCollider col = gameObject.AddComponent<SphereCollider>();
        col.radius = towerConfig.rangeRadius;
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

            if (enemy == null || enemy.GetComponent<Enemy>().IsDead())
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }

            float progress = enemy.GetComponent<Enemy>().GetProgress();

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
        GameObject ammoObject = Instantiate(towerConfig.ammoPrefab, ammoSpawnPoint.position, ammoSpawnPoint.rotation, transform);
        Ammo ammo = ammoObject.GetComponent<Ammo>();
        ammo.SetTarget(target);

    }

    public void OnPlaced(ITile tile)
    {
        this.tile = tile;
        rangeObject.SetActive(false);
        CreateCollider();
        isPlaced = true;

        value = towerConfig.cost;
        CurrencyManager.Instance.RemoveMoney(towerConfig.cost);
    }

    public bool IsPlaced() {  return isPlaced; }
    public void AddValue(int extraValue) {  value += extraValue; } // in updates
    public int GetCost() { return towerConfig.cost; }
    public int GetValue() {  return value; }

    public void OnClick()
    {
        if (isBlocked) return;
        Debug.Log("Tower clicked");
        rangeObject.SetActive(true);
    }

    public void OnUnClick()
    {
        rangeObject.SetActive(false);
    }

    public void OnRemoved()
    {
        tile.Remove();
        CurrencyManager.Instance.OnTowerRemoved(value);
        Destroy(gameObject);
    }

    public Vector3 GetPlacingOffset()
    {
        return towerConfig.placingOffset;
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