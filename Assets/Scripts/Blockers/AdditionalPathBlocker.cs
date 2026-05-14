using UnityEngine;

public abstract class AdditionalPathBlocker : MonoBehaviour
{
    protected bool isBlocked;
    
    protected virtual void OnEnable()
    {
        isBlocked = false;
        EventManager.Instance.OnAdditionalPathPlacingStarted += BlockMenu;
        EventManager.Instance.OnAdditionalPathPlacingCompleted += UnblockMenu;
    }
    
    protected virtual void OnDisable() 
    {
        EventManager.Instance.OnAdditionalPathPlacingStarted -= BlockMenu;
        EventManager.Instance.OnAdditionalPathPlacingCompleted -= UnblockMenu;
    }
    
    private void BlockMenu()
    {
        isBlocked = true;
    }
    
    private void UnblockMenu()
    {
        isBlocked = false;
    }
}