using System.Collections.Generic;
using UnityEngine;

public class LockSFXManager : SFXManagerNetworkExtension {
    [SerializeField]
    private AudioSource unlockedSFX;

    void Awake() {
        SFXDictionary = new Dictionary<AudioSource, bool> {
            { unlockedSFX, unlockedSFX.gameObject.activeInHierarchy }
        };

        SetActiveAll(false);
    }

    public void SetUnlocked(bool _isActive) {
        int index = SetActiveAudioSource(unlockedSFX, _isActive);

        NotifySFXSet(index, _isActive);
    }
}
