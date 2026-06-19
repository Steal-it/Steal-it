using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseNetworkObjectMessage {
    public string Type;

    public BaseNetworkObjectMessage(string _type) {
        Type = _type;
    }
}

[Serializable]
public class MovementMessage : BaseNetworkObjectMessage {
    public const string TYPE = "MovementMessage";

    public Pose Pose;
    public bool IsOwned;

    public MovementMessage(Pose _pose, bool _isOwned) : base(TYPE) {
        Pose = _pose;
        IsOwned = _isOwned;
    }
}

[Serializable]
public class AnimationMessage : BaseNetworkObjectMessage {
    public const string TYPE = "AnimationMessage";

    public Dictionary<string, object> ParameterDictionary;

    public AnimationMessage() : base(TYPE) {
        ParameterDictionary = new Dictionary<string, object>();
    }
}
