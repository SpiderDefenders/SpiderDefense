using UnityEngine.EventSystems;

public class RemoveTowerUI : InGameButtonUI
{
    private TowerMenu towerMenu;

    private void Start()
    {
        towerMenu = FindAnyObjectByType<TowerMenu>();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        towerMenu.RemoveTower();

    }
}