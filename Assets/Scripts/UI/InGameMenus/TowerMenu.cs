using UnityEngine;

public class TowerMenu : InGameMenu
{
    private Tower selectedTower;
    private void Start()
    {
        Init();
        toggleButton.gameObject.SetActive(false);
        moveBackOnDown = true;
    }

    public void OpenMenu(Tower tower)
    {
        selectedTower = tower;
        toggleButton.gameObject.SetActive(true);
        gameObject.transform.SetAsLastSibling();
        ToggleMenu(true);
    }

    public void InstantOpenMenu(Tower tower)
    {
        selectedTower = tower;
        toggleButton.gameObject.SetActive(true);
        gameObject.transform.SetAsLastSibling();
        menuPanel.transform.position = new Vector3(
            menuPanel.transform.position.x,
            visibleY,
            menuPanel.transform.position.z
        );
        isOpen = true;
    }

    public override void CloseEverything()
    {
        toggleButton.gameObject.SetActive(false);
        base.CloseEverything();
        if (selectedTower != null) {
            selectedTower.OnUnClick();
        }
        selectedTower = null;
    }

    public void RemoveTower()
    {
        selectedTower.OnRemoved();
        CloseEverything();
    }
}