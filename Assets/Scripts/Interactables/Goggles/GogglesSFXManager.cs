using System.Collections.Generic;
using UnityEngine;

public class GogglesSFXManager : SFXManagerNetworkExtension {
    [SerializeField]
    private AudioSource activationSFX;
    [SerializeField]
    private AudioSource deactivationSFX;

    void Awake() {
        SFXDictionary = new Dictionary<AudioSource, bool> {
            { activationSFX, activationSFX.gameObject.activeInHierarchy },
            { deactivationSFX, deactivationSFX.gameObject.activeInHierarchy }
        };

        SetActiveAll(false);
    }

    public void SetActivation(bool _isActive) {
        int index = SetActiveAudioSource(activationSFX, _isActive);

        NotifySFXSet(index, _isActive);
    }

    public void SetDeactivation(bool _isActive) {
        int index = SetActiveAudioSource(deactivationSFX, _isActive);

        NotifySFXSet(index, _isActive);
    }
}
