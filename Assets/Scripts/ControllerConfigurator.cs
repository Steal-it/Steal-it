using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ControllerConfigurator : MonoBehaviour, IControllerConfigurator {
    [SerializeField]
    private NearFarInteractor nearFarInteractor;
    [SerializeField]
    private InputActionReference controllerActivateInputAction;

    public void Enable(Action<InputAction.CallbackContext> _action) {
        controllerActivateInputAction.action.Enable();
        controllerActivateInputAction.action.performed += _action;

        // TODO: track device change https://youtu.be/NObwdF9RqCg?si=QtYNqLSqe-xpNHuo
    }

    public void Disable(Action<InputAction.CallbackContext> _action) {
        controllerActivateInputAction.action.Disable();
        controllerActivateInputAction.action.performed -= _action;
    }
}
