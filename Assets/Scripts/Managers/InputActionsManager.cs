using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionsManager : MonoBehaviour {
    [Serializable]
    class InputActionsAssociation {
        public InputActionReference input;
        public CustomAction customAction;
    }
    [SerializeField]
    private InputActionsAssociation[] inputActionsAssociations;

    void Awake() {
        foreach (var association in inputActionsAssociations) {
            association.input.action.performed += association.customAction.OnInputFired;
            association.input.action.canceled += association.customAction.OnInputStop;
            association.customAction.OnInputSetActive += (_value) => ToggleInputActivation(association.input.action, _value);
        }
    }

    void OnDestroy() {
        foreach (var association in inputActionsAssociations) {
            association.input.action.performed -= association.customAction.OnInputFired;
            association.input.action.canceled -= association.customAction.OnInputStop;
        }
    }

    private void ToggleInputActivation(InputAction inputAction, bool _active) {
        if (_active) {
            inputAction.Enable();
        } else {
            inputAction.Disable();
        }
    }
}
