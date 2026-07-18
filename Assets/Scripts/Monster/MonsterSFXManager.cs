using System.Collections.Generic;
using UnityEngine;

public class MonsterSFXManager : SFXManagerNetworkExtension {
    [SerializeField]
    private AudioSource wanderSFX;
    [SerializeField]
    private AudioSource chaseSFX;
    [SerializeField]
    private AudioSource murderSFX;
    [SerializeField]
    private AudioSource flashedSFX;

    void Awake() {
        SFXDictionary = new Dictionary<AudioSource, bool> {
            { wanderSFX, wanderSFX.gameObject.activeInHierarchy },
            { chaseSFX, chaseSFX.gameObject.activeInHierarchy },
            { murderSFX, murderSFX.gameObject.activeInHierarchy },
            { flashedSFX, flashedSFX.gameObject.activeInHierarchy }
        };

        SetActiveAll(false);
    }

    public void SetWander(bool _isActive) {
        int index = SetActiveAudioSource(wanderSFX, _isActive);

        NotifySFXSet(index, _isActive);
    }

    public void SetChase(bool _isActive) {
        int index = SetActiveAudioSource(chaseSFX, _isActive);

        NotifySFXSet(index, _isActive);
    }

    public void SetMurder(bool _isActive) {
        int index = SetActiveAudioSource(murderSFX, _isActive);

        NotifySFXSet(index, _isActive);
    }

    public void SetFlashed(bool _isActive) {
        int index = SetActiveAudioSource(flashedSFX, _isActive);

        NotifySFXSet(index, _isActive);
    }
}
