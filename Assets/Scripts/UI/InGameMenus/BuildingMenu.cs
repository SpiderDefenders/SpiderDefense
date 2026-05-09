using UnityEngine;

public class BuildingMenu : InGameMenu
{
    
    private void OnEnable()
    {
        EventManager.Instance.OnAdditionalPathPlacingCompleted += RestoreMenuToClosedState;
    }
    
    private void OnDisable()
    {
        EventManager.Instance.OnAdditionalPathPlacingCompleted -= RestoreMenuToClosedState;
    }

    private void RestoreMenuToClosedState()
    {
        RestoreHiddenMenu();
    }
}