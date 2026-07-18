using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Torch : CustomAction {
    public event EventHandler<OnTorchTurnedEventArgs> OnTorchTurned;
    public class OnTorchTurnedEventArgs : EventArgs {
        public bool IsTurnedOn;
        public bool IsBatteryRunOut;
    }

    [SerializeField]
    private XRSocketInteractor socketInteractor;
    [SerializeField]
    private TorchLight torchLight;

    private Battery battery;
    private bool emitLight;
    private bool canUpgrade;

    void Start() {
        socketInteractor.hoverEntered.AddListener(OnNewBatteryInstalled);
        socketInteractor.selectEntered.AddListener(OnNewBatteryInstalled);
        socketInteractor.selectExited.AddListener(RemoveBattery);

        OnTorchTurned += torchLight.ToggleLight;

        // Start the torch with no batteries, so turn it off
        emitLight = false;
        ToggleLight();
    }

    private void OnNewBatteryInstalled(HoverEnterEventArgs _event) {
        if (battery && canUpgrade) {
            // If a battery is already present I add recharge it
            Battery additionalBattery = _event.interactableObject.transform.GetComponent<Battery>();
            battery?.Recharge(additionalBattery.chargeLevel);
            Destroy(additionalBattery.gameObject);
        }
        if (!canUpgrade) {
            canUpgrade = true;
        }
    }

    private void OnNewBatteryInstalled(SelectEnterEventArgs _event) {
        battery = _event.interactableObject.transform.GetComponent<Battery>();
        battery.OnBatteryRanOut += Battery_OnBatteryRanOut;

        // Start discharging the battery
        battery.BatteryInserted();

        // Turn on the light
        emitLight = true;
        ToggleLight();

        // Disallow the socket of the battery to show the mesh of a new battery
        socketInteractor.showInteractableHoverMeshes = false;
        canUpgrade = false;

        battery.GetComponent<NetworkMovement>().SelectObject(); // become owner and sender of the battery
    }

    private void RemoveBattery(SelectExitEventArgs _) {
        if (battery == null) return;

        emitLight = false;
        ToggleLight();

        battery.BatteryRemoved();

        battery.GetComponent<NetworkMovement>().DeselectObject();

        battery = null;

        // Allow the socket of the battery to show the mesh of a new battery
        socketInteractor.showInteractableHoverMeshes = true;
    }

    // UNUSED METHOD FOR SHAKE   
    public void RemoveBattery(Vector3 dropoutVelocity) {
        if (battery == null) return;

        emitLight = false;
        ToggleLight();

        socketInteractor.enabled = false;

        battery.Drop(dropoutVelocity);
        battery = null;

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
        ToggleLight(true);
    }

    private void ToggleLight(bool _isBatteryRunOut = false) {
        OnTorchTurned?.Invoke(this, new OnTorchTurnedEventArgs {
            IsTurnedOn = emitLight,
            IsBatteryRunOut = _isBatteryRunOut
        });
    }

    public void ToggleInPocket(bool _isInPocket) {
        if (battery == null) return;
        if (_isInPocket) {
            battery.GetComponentInChildren<BatteryVisuals>().Disable();
            battery.StopUse();
        } else {
            battery.GetComponentInChildren<BatteryVisuals>().Enable();
            battery.Use();
        }
    }

    void OnDestroy() {
        socketInteractor.hoverEntered.RemoveAllListeners();
    }

    protected override void AfterInputSet(bool _isActive) {
        if (!socketInteractor.hasSelection) {
            socketInteractor.allowSelect = _isActive;
        }
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

    public override void OnInputStop(InputAction.CallbackContext _) { }
}
