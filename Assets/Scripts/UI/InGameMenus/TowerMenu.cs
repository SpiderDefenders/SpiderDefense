using UnityEngine;

public class TowerMenu : InGameMenu
{
    private Tower selectedTower;
    private RemoveTowerUI removeTowerUI;
    private void Start()
    {
        Init();
        toggleButton.gameObject.SetActive(false);
        moveBackOnDown = true;
        removeTowerUI = GetComponentInChildren<RemoveTowerUI>();
    }

    public void OpenMenu(Tower tower)
    {
        BaseOpen(tower);
        ToggleMenu(true);
    }

    public void InstantOpenMenu(Tower tower)
    {
        BaseOpen(tower);
        menuPanel.transform.position = new Vector3(
            menuPanel.transform.position.x,
            visibleY,
            menuPanel.transform.position.z
        );
        isOpen = true;
    }

    private void BaseOpen(Tower tower)
    {
        selectedTower = tower;
        toggleButton.gameObject.SetActive(true);
        gameObject.transform.SetAsLastSibling();
        removeTowerUI.SetAmount(CurrencyManager.Instance.GetMoneyOnTowerRemoved(tower.GetValue()));
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