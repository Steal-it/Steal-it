using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CustomAction : MonoBehaviour {
    public event Action<bool> OnInputSetActive;

    public abstract void OnInputFired(InputAction.CallbackContext ctx);
    public abstract void OnInputStop(InputAction.CallbackContext ctx);

    protected void InputSetActive(bool isActive) {
        OnInputSetActive?.Invoke(isActive);
    }
}
