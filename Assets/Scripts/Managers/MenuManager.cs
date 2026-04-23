using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    
    private TowerMenu towerMenu;
    private BuildingMenu buildingMenu;
    
    void Start()
    {
        towerMenu = FindAnyObjectByType<TowerMenu>();
        buildingMenu = FindAnyObjectByType<BuildingMenu>();
    }

    public void CloseAll()
    {
        towerMenu.CloseEverything();
        buildingMenu.CloseEverything();
    }

    public void OpenTowerMenu(Tower tower)
    {
        if (buildingMenu.IsOpen())
        {
            towerMenu.InstantOpenMenu(tower);
            buildingMenu.CloseEverything();
        }
        else
        {
            towerMenu.OpenMenu(tower);
        }
        
    }

}
