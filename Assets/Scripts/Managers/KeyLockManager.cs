using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class KeyLockManager : MonoBehaviour {
    [SerializeField]
    private XRBaseInteractable lockedInteractable;
    [SerializeField]
    private Lock[] locks;

    private Rigidbody lockedRigidbody;

    void Start() {
        if (lockedInteractable.TryGetComponent(out lockedRigidbody)) {
            lockedRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
        lockedInteractable.enabled = false;
        foreach (var l in locks) {
            l.AddListener(TryToUnlock);
        }
    }

    private void TryToUnlock() {
        foreach (var l in locks) {
            if (!l.IsUnlocked()) return;
        }
        lockedInteractable.enabled = true;
        if (lockedRigidbody != null) {
            lockedRigidbody.constraints = RigidbodyConstraints.None;
        }
    }

}
