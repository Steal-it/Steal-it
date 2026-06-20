using System;
using System.Collections.Generic;
using Ubiq.Dictionaries;
using Unity.XR.CoreUtils.Collections;
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
    #region Animator Parameters
    // [Serializable]
    // public class AnimatorParameter {
    //     public string Name;

    //     public AnimatorParameter(string _name) {
    //         Name = _name;
    //     }
    // }

    public interface IAnimatorParameter { }

    [Serializable]
    public class AnimatorBoolParameter : IAnimatorParameter {
        public bool Value;

        public AnimatorBoolParameter(bool _value) {
            Value = _value;
        }
    }
    #endregion

    public const string TYPE = "AnimationMessage";

    // public SerializableDictionary<string, IAnimatorParameter> ParameterDictionary;
    public SerializableDictionary<string, bool> ParameterDictionary;

    public AnimationMessage() : base(TYPE) {
        // ParameterDictionary = new SerializableDictionary<string, IAnimatorParameter>();
        ParameterDictionary = new SerializableDictionary<string, bool>();
    }
}
