using System;
using UnityEngine.InputSystem;

[Serializable]
public sealed class InputActionsAssociation {
    public InputActionReference input;
    public CustomAction primaryAction;

    private CustomAction activeAction;

    public void Init() {
        activeAction = primaryAction;
        Enable();
    }

    public void ChangeCurrentAction(CustomAction _action) {
        activeAction = _action ?? primaryAction;
        Disable();
        Enable();
    }

    private void Enable() {
        input.action.performed += activeAction.InputFired;
        input.action.canceled += activeAction.InputStop;
    }

    public void Disable() {
        input.action.performed -= activeAction.InputFired;
        input.action.canceled -= activeAction.InputStop;
    }
}
