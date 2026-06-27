using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GogglesSocket : CustomAction {
    [SerializeField]
    private GameObject goggleVisual;

    private AvatarLocale avatar;
    private NetworkObjectEnabler networkObjectEnabler;

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
        TryGetComponent(out networkObjectEnabler);
        TryGetComponent(out avatar);
        seeThrough = FindFirstObjectByType<SeeThrough>();
    }

    void Start() {
        if (!avatar.IsLocal()) {
            networkObjectEnabler.OnMessageReceived += EnableGogglesAvatarVisual;
            socketInteractor.gameObject.SetActive(false);
            return;
        }
        socketInteractor.selectEntered.AddListener(OnGogglesInserted);
    }

    private void EnableGogglesAvatarVisual(bool _isActive) {
        goggleVisual.SetActive(_isActive);
    }

    private void OnGogglesInserted(SelectEnterEventArgs _event) {
        currentGoogles = _event.interactableObject.transform.GetComponent<Goggles>();
        currentGoogles.OnGooglesToggle += ToggleSeeThrough;
        currentGoogles.transform.SetParent(transform, false);
        currentGoogles.DisableVisuals();
        networkObjectEnabler.SendEnableParameters(true);
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
