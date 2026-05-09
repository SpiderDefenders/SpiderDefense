using UnityEngine;

public class MenuManager : AdditionalPathBlocker
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
        towerMenu.CompletelyHideMenu();
        buildingMenu.CloseEverything();
        buildingMenu.CompletelyHideMenu();
    }

    public void OpenTowerMenu(Tower tower)
    {
        if (isBlocked) return;
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
