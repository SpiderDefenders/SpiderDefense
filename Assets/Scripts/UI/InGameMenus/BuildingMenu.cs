using UnityEngine;

public class BuildingMenu : InGameMenu
{
    
    private new void OnEnable()
    {
        base.OnEnable();
        EventManager.Instance.OnAdditionalPathPlacingCompleted += RestoreMenuToClosedState;
        EventManager.Instance.OnGameResumed += RestoreMenuToClosedState;
    }
    
    private new void OnDisable()
    {
        base.OnDisable();
        EventManager.Instance.OnAdditionalPathPlacingCompleted -= RestoreMenuToClosedState;
        EventManager.Instance.OnGameResumed -= RestoreMenuToClosedState;
    }

    private void RestoreMenuToClosedState()
    {
        RestoreHiddenMenu();
    }
}