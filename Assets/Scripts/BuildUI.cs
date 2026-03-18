using UnityEngine;

public class BuildUI : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;

    public void OnBuildTowerClicked()
    {
        BuildManager.Instance.StartPlacement(towerPrefab);
    }
}