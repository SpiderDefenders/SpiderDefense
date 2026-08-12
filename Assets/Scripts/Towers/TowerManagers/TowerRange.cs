using System.Collections.Generic;
using UnityEngine;

public class TowerRange : MonoBehaviour
{
    public List<GameObject> EnemiesInRange { get; private set; } = new();
    private GameObject rangeObject;
    private float yOffset = 0.05f;
    private float yScale = 0.01f;
    private Transform towerTransform;

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemiesInRange.Remove(other.gameObject);
        }
    }

    public void RefreshEnemiesInRange()
    {
        EnemiesInRange.Clear();

        SphereCollider col = GetComponent<SphereCollider>();
        Vector3 worldCenter = towerTransform.position + col.center;

        Collider[] hits = Physics.OverlapSphere(worldCenter, col.radius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemiesInRange.Add(hit.gameObject);
            }
        }
    }

    public void CreateCollider(float radius)
    {
        SphereCollider col = gameObject.AddComponent<SphereCollider>();
        col.radius = radius;
        col.center = new Vector3(0f, yOffset, 0f);
        col.isTrigger = true;
    }

    public void CreateRangeObject(float radius, Material rangeMaterial, Transform towerTransform)
    {
        this.towerTransform = towerTransform;
        rangeObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rangeObject.tag = "Range";
        rangeObject.name = "Range";
        rangeObject.transform.SetParent(towerTransform);
        rangeObject.transform.localPosition = new Vector3(0f, yOffset, 0f);
        rangeObject.transform.localRotation = Quaternion.identity;
        rangeObject.transform.localScale = new Vector3(radius * 2f, yScale, radius * 2f);

        rangeObject.GetComponent<MeshRenderer>().material = rangeMaterial;
    }

    public void SetActive(bool active)
    {
        rangeObject.SetActive(active);
    }

    public void UpgradeRange(float newRadius)
    {
        rangeObject.transform.localScale = new Vector3(newRadius * 2f, yScale, newRadius * 2f);
        SphereCollider col = GetComponent<SphereCollider>();
        col.radius = newRadius;

        RefreshEnemiesInRange();
    }

}