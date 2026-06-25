using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Lock : MonoBehaviour {
    private XRSocketInteractor socket;
    public bool IsUnlocked { get; private set; }
    public event Action OnUnlock;

    void Start() {
        if (TryGetComponent(out socket)) {
            socket.selectEntered.AddListener(Unlock);
        }
    }

    private void Unlock(SelectEnterEventArgs _) {
        IsUnlocked = true;
        OnUnlock?.Invoke();
        Destroy(socket.GetOldestInteractableSelected().transform.gameObject);
        Destroy(gameObject);
    }

    private void OnDisable() {
        socket.selectEntered.RemoveListener(Unlock);
    }

}
