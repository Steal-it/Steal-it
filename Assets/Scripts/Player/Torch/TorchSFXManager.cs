using System;
using System.Collections.Generic;
using Ubiq.Dictionaries;
using UnityEngine;

public class TorchSFXManager : SFXManagerNetworkExtension {
    [SerializeField]
    private AudioSource lightOnSFX;
    [SerializeField]
    private AudioSource lightOffSFX;
    [SerializeField]
    private AudioSource batteryRunOutSFX;

    private Transform parent;

    void Awake() {
        SFXDictionary = new Dictionary<AudioSource, bool> {
            { lightOnSFX, lightOnSFX.gameObject.activeInHierarchy },
            { lightOffSFX, lightOffSFX.gameObject.activeInHierarchy },
            { batteryRunOutSFX, batteryRunOutSFX.gameObject.activeInHierarchy }
        };

        SetActiveAll(false);
    }

    private void NotifyAvatarSFXChange(int _idx, bool _isActive) {
        SerializableDictionary _SFXDictionary = new SerializableDictionary(new Dictionary<string, string> {
            { _idx.ToString(), _isActive.ToString() }
        });

        NetworkReferenceManager.Instance.MessageHandler.SendAvatarTorchSFXMessage(_SFXDictionary);
    }

    private void NotifyAvatarSFXChange() {
        NetworkReferenceManager.Instance.MessageHandler.SendAvatarTorchSFXMessage(SerializeSFXDictionary());
    }

    public void SetLightOn(bool _isActive) {
        int index = SetActiveAudioSource(lightOnSFX, _isActive);

        NotifyAvatarSFXChange(index, _isActive);
    }

    public void SetLightOff(bool _isActive) {
        int index = SetActiveAudioSource(lightOffSFX, _isActive);

        NotifyAvatarSFXChange(index, _isActive);
    }

    public void SetBatteryRunOut(bool _isActive) {
        int index = SetActiveAudioSource(batteryRunOutSFX, _isActive);

        NotifyAvatarSFXChange(index, _isActive);
    }

    public void SetAllSFXs(bool _isActive) {
        SetActiveAll(_isActive);

        NotifyAvatarSFXChange();
    }

    public void OnAvatarTorchSFXReceived(object _sender, MessageHandler.OnAvatarTorchSFXMessageReceivedEventArgs _event) {
        if (parent == null) {
            parent = transform;
        }

        string playerUUID = parent.name;

        while (!playerUUID.Contains("Remote Avatar") && !playerUUID.Contains("Local Avatar")) {
            parent = parent.parent;
            playerUUID = parent.name;
        }

        if (playerUUID != "Local Avatar") {
            playerUUID = playerUUID.Split('#')[1];
            if (playerUUID != _event.PlayerUUID) {
                return;
            }
        }

        SetSFXDictionary(_event.SFXDictionary);
    }
}
