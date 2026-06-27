using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class FornitureUnlockable : Unlockable {
    private Rigidbody lockedRigidbody;
    private XRBaseInteractable lockedInteractable;

    void Start() {
        TryGetComponent(out lockedInteractable);
        if (TryGetComponent(out lockedRigidbody)) {
            lockedRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
        lockedInteractable.enabled = false;
    }

    public override void Unlock() {
        if (lockedRigidbody != null) {
            lockedRigidbody.constraints = RigidbodyConstraints.None;
        }
        lockedInteractable.enabled = true;
    }
}
