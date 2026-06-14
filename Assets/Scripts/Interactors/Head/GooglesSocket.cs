using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GooglesSocket : CustomAction {
    private SeeThrough seeThrough;
    private XRSocketInteractor socketInteractor;
    private Googles currentGoogles;

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
        currentGoogles = _event.interactableObject.transform.GetComponent<Googles>();
        currentGoogles.OnGooglesToggle += ToggleSeeThrough;
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
