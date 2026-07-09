using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GogglesSocket : CustomAction {
    private SeeThrough seeThrough;
    private XRSocketInteractor socketInteractor;
    private Goggles currentGoogles;

    public override void OnInputFired(InputAction.CallbackContext _) {
        currentGoogles?.ToggleGlasses();
    }

    public override void OnInputStop(InputAction.CallbackContext _) {
    }

    void Awake() {
        TryGetComponent(out socketInteractor);
        seeThrough = FindFirstObjectByType<SeeThrough>();
    }

    void Start() {
        socketInteractor.selectEntered.AddListener(OnGogglesInserted);
    }

    private void OnGogglesInserted(SelectEnterEventArgs _event) {
        currentGoogles = _event.interactableObject.transform.GetComponent<Goggles>();
        currentGoogles.OnGooglesToggle += ToggleSeeThrough;
        currentGoogles.transform.SetParent(transform, false);
        currentGoogles.DisableVisuals();
        socketInteractor.enabled = false;
    }

    private void ToggleSeeThrough(bool _active) {
        if (_active) {
            seeThrough.EnableSeeThrough();
        } else {
            seeThrough.DisableSeeThrough();
        }
    }

    void OnDestroy() {
        socketInteractor.selectEntered.RemoveAllListeners();
    }
}
