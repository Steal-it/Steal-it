using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractNetworkObjectMessage {
    public string Type;

    public AbstractNetworkObjectMessage(string _type) {
        Type = _type;
    }
}

public class MovementMessage : AbstractNetworkObjectMessage {
    public const string TYPE = "MovementMessage";

    public Pose Pose;
    public bool IsOwned;

    public MovementMessage(Pose _pose, bool _isOwned) : base(TYPE) {
        Pose = _pose;
        IsOwned = _isOwned;
    }
}

public class AnimationMessage : AbstractNetworkObjectMessage {
    public const string TYPE = "AnimationMessage";

    public Dictionary<string, object> ParameterDictionary;

    public AnimationMessage() : base(TYPE) {
        ParameterDictionary = new Dictionary<string, object>();
    }
}
