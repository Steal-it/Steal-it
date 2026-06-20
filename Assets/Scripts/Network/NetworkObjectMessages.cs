using System;
using Ubiq.Dictionaries;
using UnityEngine;

[Serializable]
public class BaseNetworkObjectMessage {
    public string Type;

    public BaseNetworkObjectMessage(string _type) {
        Type = _type;
    }
}

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
