using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    public static event Action OnPausePressed;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.UI.Pause.performed += HandlePause;
    }

    void OnDisable()
    {
        inputActions.UI.Pause.performed -= HandlePause;
        inputActions.Disable();
    }

    private void HandlePause(InputAction.CallbackContext context)
    {
        OnPausePressed?.Invoke();
    }
}