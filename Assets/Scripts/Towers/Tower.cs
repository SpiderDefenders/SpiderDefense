using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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

    [Header("Targeting")]
    public GameObject target;
    
    [SerializeField] private Transform horizontalPivot;

    public PlacableType Type => PlacableType.Defense;
    private List<GameObject> enemiesInRange = new List<GameObject>();


    private void Awake()
    {
        CreateRangeObject();
    }
    private void Update()
    {
        if (target == null) return;
        FollowTarget();
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
            SortEnemies();
            SetTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
            SetTarget();
        }
    }

    private void SetTarget()
    {
        target = null;
        if(enemiesInRange.Count > 0)
        {
            target = enemiesInRange[0];
        }
    }

    private void SortEnemies()
    {
        // first enemy
        enemiesInRange.Sort((a, b) =>
        {
            float progressA = a.GetComponent<EnemySplineMover>().GetProgress();
            float progressB = b.GetComponent<EnemySplineMover>().GetProgress();
            return progressB.CompareTo(progressA);
        });
    }

    public void OnPlaced(ITile tile)
    {
        this.tile = tile;
        rangeObject.SetActive(false);
        CreateCollider();
    }

    //private void OnMouseDown()
    //{
    //    rangeObject.SetActive(true);
    //}

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
            horizontalPivot.rotation = targetRotation;
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