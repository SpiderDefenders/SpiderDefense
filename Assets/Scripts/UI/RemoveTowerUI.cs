using UnityEngine.EventSystems;

public class RemoveTowerUI : InGameButtonUI
{
    //[Header("UI")]
    //[SerializeField] private TextMeshProUGUI amountText;
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