using System.Collections.Generic;
using UnityEngine;

public class MonsterSFXManager : AbstractNetworkSFXManager {
    [SerializeField]
    private AudioSource wanderSFX;

    void Awake() {
        SFXDictionary = new Dictionary<AudioSource, bool> {
            { wanderSFX, wanderSFX.gameObject.activeInHierarchy }
        };

        SetActiveAll(false);
    }

    public void SetWander(bool _isActive) {
        int index = SetActiveAudioSource(wanderSFX, _isActive);

        NotifySFXSet(index, _isActive);
    }
}
