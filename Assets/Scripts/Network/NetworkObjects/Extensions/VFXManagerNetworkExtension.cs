using System;
using System.Collections.Generic;
using System.Linq;
using Ubiq.Dictionaries;
using UnityEngine;

public class VFXManagerNetworkExtension : MonoBehaviour {
    public event EventHandler<OnSFXChangedEventArgs> OnVFXChanged;
    public class OnSFXChangedEventArgs : EventArgs {
        public SerializableDictionary VFXDictionary;
    }

    protected Dictionary<ParticleSystem, bool> VFXDictionary { get; set; }

    protected int SetActiveParticleSystem(ParticleSystem _particleSystem, bool _isActive) {
        _particleSystem.gameObject.SetActive(_isActive);
        VFXDictionary[_particleSystem] = _isActive;

        return VFXDictionary.Keys.ToList().IndexOf(_particleSystem);
    }

    protected void SetActiveAll(bool _isActive) {
        List<ParticleSystem> particleSystemList = VFXDictionary.Keys.ToList();
        foreach (ParticleSystem particleSystem in particleSystemList) {
            SetActiveParticleSystem(particleSystem, _isActive);
        }
    }

    protected void NotifySFXSet(int _idx, bool _isActive) {
        SerializableDictionary _VFXDictionary = new SerializableDictionary(new Dictionary<string, string> {
            { _idx.ToString(), _isActive.ToString() }
        });

        OnVFXChanged?.Invoke(this, new OnSFXChangedEventArgs {
            VFXDictionary = _VFXDictionary
        });
    }

    protected void NotifySFXSet(Dictionary<string, string> _sfxDictionary) {
        SerializableDictionary SFXDictionary = new SerializableDictionary(_sfxDictionary);

        OnVFXChanged?.Invoke(this, new OnSFXChangedEventArgs {
            VFXDictionary = SFXDictionary
        });
    }

    public void SetSFXDictionary(SerializableDictionary _sfxDictionary) {
        foreach (KeyValuePair<string, string> entry in _sfxDictionary) {
            if (
                int.TryParse(entry.Key, out int index) &&
                bool.TryParse(entry.Value, out bool isActive)
            ) {
                ParticleSystem particleSystem = VFXDictionary.Keys.ElementAt(index);
                SetActiveParticleSystem(particleSystem, isActive);
            }
        }
    }
}