using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GooglesSocket : CustomAction {
    [SerializeField]
    private UnityEvent OnGooglesInserted;

    private SeeThrough seeThrough;
    private XRSocketInteractor socketInteractor;
    private Googles currentGoogles;

    public override void OnInputFired(InputAction.CallbackContext _) {
        currentGoogles?.ToggleGlasses();
    }

    public override void OnInputStop(InputAction.CallbackContext _) {
        return;
    }

    void Awake() {
        TryGetComponent(out socketInteractor);

        socketInteractor.selectEntered.AddListener(OnGogglesInserted);

        seeThrough = FindFirstObjectByType<SeeThrough>();
    }

    private void OnGogglesInserted(SelectEnterEventArgs _event) {
        currentGoogles = _event.interactableObject.transform.GetComponent<Googles>();
        currentGoogles.OnGooglesToggle += ToggleSeeThrough;
        OnGooglesInserted?.Invoke();
        socketInteractor.enabled = false;
        currentGoogles.DisableVisuals();
    }

    private void ToggleSeeThrough(bool _active) {
        if (_active) {
            seeThrough.EnableSeeThrough();
        } else {
            seeThrough.DisableSeeThrough();
        }
    }
}
