using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Torch : MonoBehaviour {
    public event EventHandler OnTorchDied;

    [SerializeField]
    private XRSocketInteractor socketInteractor;
    [SerializeField]
    private TorchUI torchUI;

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
        battery.StopUse();
        battery.OnBatteryRanOut -= Battery_OnBatteryRanOut;

        battery = null;
        emitLight = false;
        ToggleLight();
    }

    private void Battery_OnBatteryRanOut(object _sender, Battery.OnBatteryRanOutEventArgs _e) {
        OnTorchDied?.Invoke(this, EventArgs.Empty);

        Destroy(battery.gameObject, _e.delayToDestroy);
    }

    private void GetAndUseBattery(Transform _interactableTransform) {
        battery = _interactableTransform.GetComponent<Battery>();
        battery.OnBatteryRanOut += Battery_OnBatteryRanOut;

        StartCoroutine(battery.Use());
        StartCoroutine(UpdateUI());

        emitLight = true;
        ToggleLight();
    }

    private IEnumerator UpdateUI() {
        torchUI.ToggleDisplay(true);

        while (battery != null) {
            yield return new WaitForFixedUpdate();

            // NOTE: batter not null double check because, in the meanwhile we are waiting the fixed update,
            //       battery might have become null
            if (battery != null) {
                torchUI.UpdateBatteryDisplay(battery.ChargeLevel);
            }
        }

        // Disable UI every time a battery is removed
        torchUI.ToggleDisplay(false);
    }

    private void OnTriggerPressed(InputAction.CallbackContext _context) {
        if (battery != null) {
            emitLight = !emitLight;
            ToggleLight();
        }
    }

    private void ToggleLight() {
        Debug.Log(emitLight ? "LIGHT ON" : "LIGHT OFF");
    }

    void OnDestroy() {
        TorchManager.Instance.ControllerConfigurator.Disable(OnTriggerPressed);

        socketInteractor.selectEntered.RemoveAllListeners();
        socketInteractor.selectExited.RemoveAllListeners();
    }
}
