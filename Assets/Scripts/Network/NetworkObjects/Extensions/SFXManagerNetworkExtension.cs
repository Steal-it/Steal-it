using System;
using System.Collections.Generic;
using System.Linq;
using Ubiq.Dictionaries;
using UnityEngine;

public abstract class SFXManagerNetworkExtension : MonoBehaviour {
    public event EventHandler<OnSFXChangedEventArgs> OnSFXChanged;
    public class OnSFXChangedEventArgs : EventArgs {
        public SerializableDictionary SFXDictionary;
    }

    protected Dictionary<AudioSource, bool> SFXDictionary { get; set; }

    protected int SetActiveAudioSource(AudioSource _audioSource, bool _isActive) {
        _audioSource.gameObject.SetActive(_isActive);
        SFXDictionary[_audioSource] = _isActive;

        return SFXDictionary.Keys.ToList().IndexOf(_audioSource);
    }

    protected void SetActiveAll(bool _isActive) {
        List<AudioSource> audioSourceList = SFXDictionary.Keys.ToList();
        foreach (AudioSource audioSource in audioSourceList) {
            SetActiveAudioSource(audioSource, _isActive);
        }
    }

    protected void NotifySFXSet() {
        OnSFXChanged?.Invoke(this, new OnSFXChangedEventArgs {
            SFXDictionary = SerializeSFXDictionary()
        });
    }

    protected void NotifySFXSet(int _idx, bool _isActive) {
        SerializableDictionary _SFXDictionary = new SerializableDictionary(new Dictionary<string, string> {
            { _idx.ToString(), _isActive.ToString() }
        });

        OnSFXChanged?.Invoke(this, new OnSFXChangedEventArgs {
            SFXDictionary = _SFXDictionary
        });
    }

    protected SerializableDictionary SerializeSFXDictionary() {
        Dictionary<string, string> convertedDictionary = new Dictionary<string, string>();
        for (int i = 0; i < SFXDictionary.Count; i++) {
            convertedDictionary.Add(i.ToString(), SFXDictionary.Values.ElementAt(i).ToString());
        }

        return new SerializableDictionary(convertedDictionary);
    }

    public void SetSFXDictionary(SerializableDictionary _sfxDictionary) {
        foreach (KeyValuePair<string, string> entry in _sfxDictionary) {
            if (
                int.TryParse(entry.Key, out int index) &&
                bool.TryParse(entry.Value, out bool isActive)
            ) {
                AudioSource audioSource = SFXDictionary.Keys.ElementAt(index);
                SetActiveAudioSource(audioSource, isActive);
            }
        }
    }
}
