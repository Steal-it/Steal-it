using System;
using System.Collections.Generic;
using Ubiq.Dictionaries;
using UnityEngine;

[Serializable]
public class MovementMessage {
    public Pose Pose;
    public bool IsOwned;

    public MovementMessage(Pose _pose, bool _isOwned) {
        Pose = _pose;
        IsOwned = _isOwned;
    }
}

[Serializable]
public class AnimationMessage {
    public SerializableDictionary ParameterDictionary;

    public AnimationMessage(SerializableDictionary _parameterDictionary) {
        ParameterDictionary = _parameterDictionary;
    }
}

[Serializable]
public class IncrementCounterMessage {
    public short newCounterValue;

    public IncrementCounterMessage(short _newCounterValue) {
        newCounterValue = _newCounterValue;
    }
}

[Serializable]
public class EnabledMessage {
    public bool isActive;

    public EnabledMessage(bool _isActive) {
        isActive = _isActive;
    }
}

[Serializable]
public class AudioMessage {
    public SerializableDictionary SFXDictionary;

    public AudioMessage(SerializableDictionary _SFXDictionary) {
        SFXDictionary = _SFXDictionary;
    }
}

[Serializable]
public class ParticlesMessage {
    public SerializableDictionary VFXDictionary;

    public ParticlesMessage(SerializableDictionary _VFXDictionary) {
        VFXDictionary = _VFXDictionary;
    }
}
