using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerMenu : InGameMenu
{
    [Header("Components")]
    [SerializeField] private Image towerImage;
    [SerializeField] private TMP_Text towerNameText;
    [SerializeField] private TMP_Text deleteText;
    [SerializeField] private TMP_Text modeText;


    private Tower selectedTower;
    private TowerModeManager selectedTowerModeManager;
    private void Start()
    {
        Init();
        toggleButton.gameObject.SetActive(false);
        moveBackOnDown = true;
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
        selectedTowerModeManager = tower.GetTowerModeManager();
        toggleButton.gameObject.SetActive(true);
        gameObject.transform.SetAsLastSibling();
        SetTowerValues();
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

    private void SetTowerValues()
    {
        towerNameText.text = selectedTower.GetName();
        towerImage.sprite = selectedTower.GetImage();
        modeText.text = selectedTowerModeManager.GetCurrentMode().ToString();
        deleteText.text = CurrencyManager.Instance.GetMoneyOnTowerRemoved(selectedTower.GetValue()).ToString();
    }

    public void RemoveTower()
    {
        selectedTower.OnRemoved();
        CloseEverything();
    }

    public void NextMode()
    {
        selectedTowerModeManager.Next();
        modeText.text = selectedTowerModeManager.GetCurrentMode().ToString();
    }

    public void PreviousMode()
    {
        selectedTowerModeManager.Previous();
        modeText.text = selectedTowerModeManager.GetCurrentMode().ToString();
    }
}