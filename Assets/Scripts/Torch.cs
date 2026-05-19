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

    private Battery battery;
    private bool emitLight;

    void Start() {
        TorchManager.Instance.ControllerConfigurator.Enable(OnTriggerPressed);

        socketInteractor.selectEntered.AddListener(OnNewBatteryInstalled);
        socketInteractor.selectExited.AddListener(OnBatteryRemoved);

        // Use the initial battery at the start of the game
        if (socketInteractor.startingSelectedInteractable != null) {
            GetAndUseBattery(socketInteractor.startingSelectedInteractable.transform);
        }
    }

    private void OnNewBatteryInstalled(SelectEnterEventArgs _event) {
        GetAndUseBattery(_event.interactableObject.transform);
    }

    private void OnBatteryRemoved(SelectExitEventArgs _event) {
        if (battery == null) return;

        battery.StopUse();
        battery.OnBatteryRanOut -= Battery_OnBatteryRanOut;

        emitLight = false;
        ToggleLight();
    }

    private void Battery_OnBatteryRanOut(object _sender, EventArgs _event) {
        battery = null;

        emitLight = false;
        ToggleLight();
    }

    private void GetAndUseBattery(Transform _interactableTransform) {
        battery = _interactableTransform.GetComponent<Battery>();
        battery.OnBatteryRanOut += Battery_OnBatteryRanOut;

        battery.Use();

        emitLight = true;
        ToggleLight();
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
