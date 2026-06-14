using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CustomAction : MonoBehaviour {
    private bool isActionActive;

    public abstract void OnInputFired(InputAction.CallbackContext _ctx);
    public abstract void OnInputStop(InputAction.CallbackContext _ctx);

    public void InputSetActive(bool _isActive) {
        isActionActive = _isActive;
        AfterInputSet(_isActive);
    }

    protected virtual void AfterInputSet(bool _isActive) { }

    public void InputFired(InputAction.CallbackContext _ctx) {
        if (!isActionActive) return;
        OnInputFired(_ctx);
    }

    public void InputStop(InputAction.CallbackContext _ctx) {
        if (!isActionActive) return;
        OnInputStop(_ctx);
    }
}
