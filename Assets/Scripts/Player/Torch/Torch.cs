using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Torch : CustomAction /*, IHandComponent*/ {
    public event EventHandler<OnTorchTurnedEventArgs> OnTorchTurned;
    public class OnTorchTurnedEventArgs : EventArgs {
        public bool isTurnedOn;
    }

    [SerializeField]
    private XRSocketInteractor socketInteractor;
    [SerializeField]
    private TorchLight torchLight;

    private Battery battery;
    private bool emitLight;
    private XRInteractionManager interactionManager;

    void Start() {

        interactionManager = socketInteractor.interactionManager;
        socketInteractor.selectEntered.AddListener(OnNewBatteryInstalled);

        OnTorchTurned += torchLight.ToggleLight;

        // Start the torch with no batteries, so turn it off
        emitLight = false;
        ToggleLight();
    }

    private void OnNewBatteryInstalled(SelectEnterEventArgs _event) {
        if (battery) { // if a battery is already present i add recharge it
            Battery additionalBattery = _event.interactableObject.transform.GetComponent<Battery>();
            battery.Recharge(additionalBattery.chargeLevel);
        } else {
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
    }

    public void RemoveBattery(Vector3 dropoutVelocity) {
        if (battery == null) return;

        // if (socketInteractor != null) {
        //     interactionManager.SelectExit(
        //         (IXRSelectInteractor)socketInteractor,
        //         battery.GetComponent<XRGrabInteractable>());
        // }

        socketInteractor.enabled = false;

        battery.Drop(dropoutVelocity);

        StartCoroutine(WaitForSocketEnable());
    }

    private IEnumerator WaitForSocketEnable() {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForFixedUpdate();
        socketInteractor.enabled = true;
    }

    private void Battery_OnBatteryRanOut(object _sender, EventArgs _event) {
        battery = null;

        // Turn off the light
        emitLight = false;
        ToggleLight();

        // Allow the socket of the battery to show the mesh of a new battery
        socketInteractor.showInteractableHoverMeshes = true;
    }

    private void ToggleLight() {
        OnTorchTurned?.Invoke(this, new OnTorchTurnedEventArgs {
            isTurnedOn = emitLight
        });
    }

    void OnDestroy() {
        socketInteractor.selectEntered.RemoveAllListeners();
        socketInteractor.selectExited.RemoveAllListeners();
    }

    public override void OnInputFired(InputAction.CallbackContext _) {
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

    public override void OnInputStop(InputAction.CallbackContext _) {
    }
}
