using TMPro;
using UnityEngine;

public class HpUI : MonoBehaviour 
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI hpText;

    private void OnEnable()
    {
        EventManager.Instance.OnHPChanged += HandleHPUpdate;
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null) return;

        EventManager.Instance.OnHPChanged -= HandleHPUpdate;
    }

    private void HandleHPUpdate(int hp)
    {
        hpText.text = hp.ToString();
    }
}