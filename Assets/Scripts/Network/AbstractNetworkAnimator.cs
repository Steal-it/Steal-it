using System;
using System.Collections.Generic;
using Ubiq.Dictionaries;
using UnityEngine;

public abstract class AbstractNetworkAnimator : MonoBehaviour {
    public event EventHandler<OnAnimationChangedEventArgs> OnAnimationChanged;
    public class OnAnimationChangedEventArgs : EventArgs {
        public SerializableDictionary ParameterDictionary;
    }

    protected Animator Animator { get; set; }
    protected Dictionary<string, IAnimationParameter> ParameterTypeDictionary { get; set; }

    protected void NotifyParameterSet(string _name, string _value) {
        SerializableDictionary parameterDictionary = new SerializableDictionary();
        parameterDictionary.Update(_name, _value.ToString());

        OnAnimationChanged?.Invoke(this, new OnAnimationChangedEventArgs {
            ParameterDictionary = parameterDictionary
        });
    }

    protected void NotifyParameterSet(Dictionary<string, string> _parameterDictionary) {
        SerializableDictionary parameterDictionary = new SerializableDictionary();
        foreach (KeyValuePair<string, string> entry in _parameterDictionary) {
            parameterDictionary.Update(entry.Key, entry.Value.ToString());
        }

        OnAnimationChanged?.Invoke(this, new OnAnimationChangedEventArgs {
            ParameterDictionary = parameterDictionary
        });
    }

    public void SetParameterDictionary(SerializableDictionary _parameterDictionary) {
        foreach (KeyValuePair<string, string> entry in _parameterDictionary) {
            if (ParameterTypeDictionary.ContainsKey(entry.Key)) {
                ParameterTypeDictionary[entry.Key].TrySet(entry.Key, entry.Value, Animator);
            }
        }
    }
}