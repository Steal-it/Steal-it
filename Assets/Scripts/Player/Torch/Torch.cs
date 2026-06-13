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
    private Transform lightEmitPointTransform;
    [SerializeField, Range(0.05f, 0.5f)]
    private float lightRadius = 0.3f;
    [SerializeField, Range(3, 10)]
    private float maxLightDistance = 5;

    private Battery battery;
    private bool emitLight;
    private MonsterAI monster;

    void Start() {
        TorchManager.Instance.ControllerConfigurator.Enable(transform, OnTriggerPressed);

        socketInteractor.selectEntered.AddListener(OnNewBatteryInstalled);
        socketInteractor.selectExited.AddListener(OnBatteryRemoved);

        // Start the torch with no batteries, so turn it off
        emitLight = false;
        ToggleLight();
    }

    void Update() {
        if (!emitLight) {
            // If the torch is off and the player was flashing the monster ...
            if (monster != null) {
                // ... stop the monster light exposure
                monster.StopLightExposureTimer();
            }

            // The monster is not flashed anymore by the player
            monster = null;

            return;
        }

        // Check every collision with the light
        RaycastHit[] hitArray = Physics.SphereCastAll(lightEmitPointTransform.position, lightRadius, lightEmitPointTransform.forward, maxLightDistance);
        bool isMonsterHit = false;
        foreach (RaycastHit hit in hitArray) {
            if (hit.transform.TryGetComponent(out MonsterAI _monster)) {
                isMonsterHit = true;

                // The first time the player illuminates the monster ...
                if (monster == null) {
                    // ... start its light exposure time ...
                    monster = _monster;
                    monster.StartLightExposureTimer();
                }
            }
        }

        // ... otherwise, stop it if the monster was flashed (the torch is on but it is not illuminating the monster anymore)
        if (!isMonsterHit) {
            if (monster != null) {
                monster.StopLightExposureTimer();
                monster = null;
            }
        }
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

    void OnDrawGizmos() {
        int sphereCount = 5;
        float sphereOffset = maxLightDistance / sphereCount;
        for (int i = 1; i < sphereCount + 1; i++) {
            Gizmos.DrawWireSphere(lightEmitPointTransform.position + lightEmitPointTransform.forward * sphereOffset * i, lightRadius);
        }
    }

    void OnDestroy() {
        TorchManager.Instance.ControllerConfigurator.Disable(OnTriggerPressed);

        socketInteractor.selectEntered.RemoveAllListeners();
        socketInteractor.selectExited.RemoveAllListeners();
    }
}
