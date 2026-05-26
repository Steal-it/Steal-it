using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TorchControllerConfigurator : MonoBehaviour, ITorchControllerConfigurator {

    private enum Side {
        Left,
        Right
    }

    public Transform HandTorchControllerTransform => handTorchController.transform;

    [SerializeField]
    private Side side;
    [SerializeField]
    private HandTorchController handTorchController;
    [SerializeField]
    private InputActionReference controllerActivateInputAction;

    private NearFarInteractor nearFarInteractor;

    public void ConfigureTorchHandController() {
        handTorchController.Configure(this);
    }

    public void Enable(Transform _torchTransform, Action<InputAction.CallbackContext> _action) {
        DisableInteractions();

        controllerActivateInputAction.action.Enable();
        controllerActivateInputAction.action.performed += _action;

        // TODO: track device change https://youtu.be/NObwdF9RqCg?si=QtYNqLSqe-xpNHuo

        handTorchController.Enable(_torchTransform);
    }

    private void Start() {
        XROrigin origin = FindFirstObjectByType<XROrigin>();
        if (side == Side.Left) {
            nearFarInteractor = origin.transform.Find("Camera Offset/Left Controller").GetComponentInChildren<NearFarInteractor>();
        } else {
            nearFarInteractor = origin.transform.Find("Camera Offset/Right Controller").GetComponentInChildren<NearFarInteractor>();
        }
    }

    public void Disable(Action<InputAction.CallbackContext> _action) {
        EnableInteractions();

        controllerActivateInputAction.action.Disable();
        controllerActivateInputAction.action.performed -= _action;

        handTorchController.Disable();
    }

    public void EnableInteractions() {
        nearFarInteractor.enableNearCasting = true;
        nearFarInteractor.enableFarCasting = true;
    }

    public void DisableInteractions() {
        nearFarInteractor.enableNearCasting = false;
        nearFarInteractor.enableFarCasting = false;
    }
}
