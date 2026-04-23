using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManger : MonoBehaviour
{
    [SerializeField] private LayerMask tileMask;
    [SerializeField] private LayerMask towerMask;
    private MenuManager menuManager;
    private Tower selectedTower;

    private void Start()
    {
        menuManager = FindAnyObjectByType<MenuManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }

    }

    void HandleClick()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, towerMask))
        {
            Tower tower = hit.collider.GetComponentInParent<Tower>();
            HandleTowerClick(tower);
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileMask))
        {
            ExpandableTile tile = hit.collider.GetComponentInParent<ExpandableTile>();
            if (tile != null)
            {
                HandleExpandableTileClick(tile);
            }
        }
    }

    void HandleTowerClick(Tower tower)
    {
        if (!tower.IsPlaced()) return;

        if (selectedTower != null)
        {
            selectedTower.OnUnClick();
        }
        tower.OnClick();
        selectedTower = tower;

        menuManager.OpenTowerMenu(tower);
    }

    void HandleExpandableTileClick(ExpandableTile tile)
    {
        tile.OnClick();
    }
}