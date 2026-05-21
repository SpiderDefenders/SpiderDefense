using UnityEngine;
using UnityEngine.UI;

public class SliderHandleSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject zeroHandle;

    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);
        UpdateHandle(slider.value);
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(float value) => UpdateHandle(value);

    private void UpdateHandle(float value) => zeroHandle.SetActive(value == 0f);
}