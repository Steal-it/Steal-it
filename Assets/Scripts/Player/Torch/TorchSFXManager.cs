using System.Collections.Generic;
using UnityEngine;

public class TorchSFXManager : SFXManagerNetworkExtension {
    [SerializeField]
    private AudioSource lightOnSFX;
    [SerializeField]
    private AudioSource lightOffSFX;

    void Awake() {
        SFXDictionary = new Dictionary<AudioSource, bool> {
            { lightOnSFX, lightOnSFX.gameObject.activeInHierarchy },
            { lightOffSFX, lightOffSFX.gameObject.activeInHierarchy }
        };

        SetActiveAll(false);
    }

    public void SetLightOn(bool _isActive) {
        int index = SetActiveAudioSource(lightOnSFX, _isActive);

        NotifySFXSet(index, _isActive);
    }

    public void SetLightOff(bool _isActive) {
        int index = SetActiveAudioSource(lightOffSFX, _isActive);

        NotifySFXSet(index, _isActive);
    }
}
