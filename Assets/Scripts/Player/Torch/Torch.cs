using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Torch : MonoBehaviour {
    public event EventHandler<OnTorchTurnedEventArgs> OnTorchTurned;
    public class OnTorchTurnedEventArgs : EventArgs {
        public bool isTurnedOn;
    }

    [SerializeField]
    private XRSocketInteractor socketInteractor;
    [SerializeField]
    private LayerMask rungLayer;

    private Battery battery;
    private bool emitLight;

    void Start() {
        TorchManager.Instance.ControllerConfigurator.Enable(transform, OnTriggerPressed);

        socketInteractor.selectEntered.AddListener(OnNewBatteryInstalled);
        socketInteractor.selectExited.AddListener(OnBatteryRemoved);

        // Start the torch with no batteries, so turn it off
        emitLight = false;
        ToggleLight();
    }

    private void OnNewBatteryInstalled(SelectEnterEventArgs _event) {
        battery = _event.interactableObject.transform.GetComponent<Battery>();
        battery.OnBatteryRanOut += Battery_OnBatteryRanOut;

        // Start discharging the battery
        battery.Use();

        // Turn on the light
        emitLight = true;
        ToggleLight();

        // Disallow the socket of the battery to show the mesh of a new battery
        socketInteractor.showInteractableHoverMeshes = false;
    }

    private void OnBatteryRemoved(SelectExitEventArgs _event) {
        if (battery == null) return;

        // Stop discharging the battery
        battery.StopUse();
        battery.OnBatteryRanOut -= Battery_OnBatteryRanOut;

        // Turn off the light
        emitLight = false;
        ToggleLight();

        // Allow the socket of the battery to show the mesh of a new battery
        socketInteractor.showInteractableHoverMeshes = true;
    }

    private void Battery_OnBatteryRanOut(object _sender, EventArgs _event) {
        battery = null;

        // Turn off the light
        emitLight = false;
        ToggleLight();

        // Allow the socket of the battery to show the mesh of a new battery
        socketInteractor.showInteractableHoverMeshes = true;
    }

    private void OnTriggerPressed(InputAction.CallbackContext _context) {
        if (battery != null) {
            emitLight = !emitLight;
            ToggleLight();

            if (emitLight) {
                battery.Use();
            } else {
                battery.StopUse();
            }
        }
    }

    private void ToggleLight() {
        OnTorchTurned?.Invoke(this, new OnTorchTurnedEventArgs {
            isTurnedOn = emitLight
        });

        Debug.Log(emitLight ? "LIGHT ON" : "LIGHT OFF");
    }

    void OnDestroy() {
        TorchManager.Instance.ControllerConfigurator.Disable(OnTriggerPressed);

        socketInteractor.selectEntered.RemoveAllListeners();
        socketInteractor.selectExited.RemoveAllListeners();
    }
}
