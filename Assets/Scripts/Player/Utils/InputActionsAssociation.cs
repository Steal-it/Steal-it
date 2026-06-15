using System;
using UnityEngine.InputSystem;

[Serializable]
public sealed class InputActionsAssociation {
    public InputActionReference input;
    public CustomAction primaryAction;

    private CustomAction activeAction;

    public void Init() {
        Disable();
        activeAction = primaryAction;
        Enable();
    }

    public void ChangeCurrentAction(CustomAction _action) {
        Disable();
        activeAction = _action ?? primaryAction;
        Enable();
    }

    private void Enable() {
        input.action.performed += activeAction.InputFired;
        input.action.canceled += activeAction.InputStop;
    }

    public void Disable() {
        if (activeAction == null) return;
        input.action.performed -= activeAction.InputFired;
        input.action.canceled -= activeAction.InputStop;
    }
}
