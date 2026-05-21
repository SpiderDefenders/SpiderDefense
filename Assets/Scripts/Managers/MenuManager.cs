using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuManager : AdditionalPathBlocker
{
    private TowerMenu towerMenu;
    private BuildingMenu buildingMenu;
    private List<InGameMenu> inGameMenus = new();

    void Start()
    {
        inGameMenus.AddRange(FindObjectsByType<InGameMenu>(FindObjectsSortMode.None));
        towerMenu = inGameMenus.OfType<TowerMenu>().FirstOrDefault();
        buildingMenu = inGameMenus.OfType<BuildingMenu>().FirstOrDefault();
    }

    public void CloseAll()
    {
        foreach (var menu in inGameMenus)
        {
            menu.CloseEverything();
            menu.CompletelyHideMenu();
        }
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
