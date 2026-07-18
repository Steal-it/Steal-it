using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Lock : MonoBehaviour {
    public bool IsUnlocked { get; private set; }

    [SerializeField]
    private NetworkAudio networkAudio;
    [SerializeField]
    private LockSFXManager lockSFXManager;
    [SerializeField]
    private GameObject visuals;

    private XRSocketInteractor socket;
    public event Action OnUnlock;

    void Start() {
        if (TryGetComponent(out socket)) {
            socket.selectEntered.AddListener(Unlock);
        }

        lockSFXManager.OnSFXChanged += LockSFXManager_OnSFXChanged;
        networkAudio.OnMessageReceived += NetworkAudio_OnMessageReceived;
    }

    private void Unlock(SelectEnterEventArgs _) {
        IsUnlocked = true;
        lockSFXManager.SetUnlocked(true);

        OnUnlock?.Invoke();

        Destroy(socket.GetOldestInteractableSelected().transform.gameObject);
        Destroy(visuals);
    }

    private void LockSFXManager_OnSFXChanged(object _sender, SFXManagerNetworkExtension.OnSFXChangedEventArgs _event) {
        networkAudio.SendSFXs(_event.SFXDictionary);
    }

    private void NetworkAudio_OnMessageReceived(object _sender, NetworkAudio.OnMessageReceivedEventArgs _event) {
        lockSFXManager.SetSFXDictionary(_event.SFXDictionary);
    }

    private void OnDestroy() {
        socket.selectEntered.RemoveListener(Unlock);
        lockSFXManager.OnSFXChanged -= LockSFXManager_OnSFXChanged;
        networkAudio.OnMessageReceived -= NetworkAudio_OnMessageReceived;
    }
}
