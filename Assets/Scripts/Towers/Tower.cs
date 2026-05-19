using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : AdditionalPathBlocker, IPlacable, IDefense
{
    private ITile tile;

    [SerializeField] protected TowerSO towerConfig;
    [SerializeField] protected Transform ammoSpawnPoint;

    [Header("Pivoting")]
    [SerializeField] protected Transform horizontalPivot;
    [SerializeField] private Transform verticalPivot;
    [SerializeField] protected float aimTolerance = 10f;

    GameObject ammoObject;

    private GameObject rangeObject;
    protected GameObject target;
    private float yOffset = 0.05f;
    private float shootingCountdown = 0f;
    protected bool isRotatedOnTarget;
    private TowerModeManager modeManager;
    public PlacableType Type => PlacableType.Defense;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    private bool isPlaced = false;
    private bool isGameOver = false;

    private int value;

    private new void OnEnable()
    {
        base.OnEnable();
        EventManager.Instance.OnGameOver += HandleGameOver;
    }

    private new void OnDisable()
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
        modeManager = new TowerModeManager(towerConfig.shootingModes, towerConfig.startShootingMode);
        CreateRangeObject();
        ammoObject = Instantiate(towerConfig.ammoPrefab, ammoSpawnPoint.position, ammoSpawnPoint.rotation, transform);
    }
    private void Update()
    {
        if (isGameOver)
            return;
        SetTarget();

        if (target != null)
        {
            isRotatedOnTarget = true;
            FollowTarget();

            if (shootingCountdown <= 0f && isRotatedOnTarget)
            {
                Shoot();
                shootingCountdown = towerConfig.shootingCooldown;
            }
        }

        if (shootingCountdown <= towerConfig.shootingCooldown / 2 && ammoObject == null)
        {
            ammoObject = Instantiate(towerConfig.ammoPrefab, ammoSpawnPoint.position, ammoSpawnPoint.rotation, transform);
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
        float bestValue = -Mathf.Infinity;

        // go backwards to avoid errors when removing enemies
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            GameObject enemy = enemiesInRange[i];

            if (enemy == null || enemy.GetComponent<Enemy>().IsDead())
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }

            float value = modeManager.GetModeValue(enemy.GetComponent<Enemy>());

            if (value > bestValue)
            {
                bestValue = value;
                bestTarget = enemy;
            }
        }

        target = bestTarget;
    }

    private void Shoot()
    {
        //GameObject ammoObject = Instantiate(towerConfig.ammoPrefab, ammoSpawnPoint.position, ammoSpawnPoint.rotation, transform);
        Ammo ammo = ammoObject.GetComponent<Ammo>();
        ammo.SetTarget(target);
        ammoObject = null;

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
    public string GetName() { return towerConfig.towerName; }
    public Sprite GetImage() { return towerConfig.towerImage; }
    public TowerModeManager GetTowerModeManager() {return modeManager;}
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
        float distance = flatDirection.magnitude;
        float heightDifference = direction.y;

        RotateHorizontal(flatDirection);
        RotateVertical(distance, heightDifference);
    }

    private void RotateHorizontal(Vector3 flatDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
        
        if (Quaternion.Angle(horizontalPivot.rotation, targetRotation) > aimTolerance)
        {
            isRotatedOnTarget = false;
        }

        horizontalPivot.rotation = Quaternion.Slerp(
            horizontalPivot.rotation,
            targetRotation,
            Time.deltaTime * 15f
            );
    }
    
    protected virtual void RotateVertical(float distance, float height)
    {

        float baseAngle = Mathf.Atan2(height, distance) * Mathf.Rad2Deg;
        float angle = Mathf.Clamp(baseAngle + distance * 2f, 0f, 75f);

        Quaternion verticalTargetRotation = Quaternion.Euler(angle, 0f, 0f);
        float currentAngle = verticalPivot.localEulerAngles.x;
        if (currentAngle > 180f) currentAngle -= 360f;
        float error = Mathf.Abs(Mathf.DeltaAngle(currentAngle, angle));
        if (error > aimTolerance)
        {
            isRotatedOnTarget = false;
        }

        verticalPivot.localRotation = Quaternion.Slerp(
            verticalPivot.localRotation,
            verticalTargetRotation,
            Time.deltaTime * 10f
        );
    }
}