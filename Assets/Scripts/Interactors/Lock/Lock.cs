using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class Lock : MonoBehaviour {
    private XRSocketInteractor socket;
    private bool isUnlocked = false;
    private event Action onUnlock;

    void Start() {
        if (TryGetComponent(out socket)) {
            socket.selectEntered.AddListener(Unlock);
        }
    }

    private void Unlock(SelectEnterEventArgs _) {
        isUnlocked = true;
        onUnlock?.Invoke();
        Destroy(socket.GetOldestInteractableSelected().transform.gameObject);
        Destroy(gameObject);
    }


    public void AddListener(Action onUnlock) {
        this.onUnlock += onUnlock;
    }

    public bool IsUnlocked() {
        return isUnlocked;
    }


    private void OnDisable() {
        socket.selectEntered.RemoveListener(Unlock);
        onUnlock = null;
    }

}
