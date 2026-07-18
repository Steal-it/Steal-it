using System;
using Ubiq.Dictionaries;

[Serializable]
public class BaseMessage {
    public string type;

    public BaseMessage(string _type) {
        type = _type;
    }
}

[Serializable]
public class ReadyMessage : BaseMessage {
    public const string TYPE = "ReadyMessage";

    public ReadyMessage() : base(TYPE) { }
}

[Serializable]
public class LoadLevelCompletedMessage : BaseMessage {
    public const string TYPE = "LoadLevelCompletedMessage";

    public LoadLevelCompletedMessage() : base(TYPE) { }
}

[Serializable]
public class RecoverCurrentCounterRequestMessage : BaseMessage {
    public const string TYPE = "RecoverCurrentCounterRequestMessage";

    public RecoverCurrentCounterRequestMessage() : base(TYPE) { }
}

[Serializable]
public class RecoverCurrentCounterReplyMessage : BaseMessage {
    public const string TYPE = "RecoverCurrentCounterReply";

    public int localCounter;

    public RecoverCurrentCounterReplyMessage(int _localCounter) : base(TYPE) {
        localCounter = _localCounter;
    }
}

[Serializable]
public class NewClientAsServerElectionMessage : BaseMessage {
    public const string TYPE = "NewClientAsServerElectionMessage";

    public string clientAsServerUuid;

    public NewClientAsServerElectionMessage(string _clientAsServerUuid) : base(TYPE) {
        clientAsServerUuid = _clientAsServerUuid;
    }
}

[Serializable]
public class StartMonsterLightExposure : BaseMessage {
    public const string TYPE = "StartMonsterLightExposureMessage";

    public StartMonsterLightExposure() : base(TYPE) { }
}

[Serializable]
public class StopMonsterLightExposure : BaseMessage {
    public const string TYPE = "StopMonsterLightExposureMessage";

    public StopMonsterLightExposure() : base(TYPE) { }
}

[Serializable]
public class ActivateSpectatorModeMessage : BaseMessage {
    public const string TYPE = "ActivateSpectatorModeMessage";

    public string PlayerUUID;

    public ActivateSpectatorModeMessage(string _playerUUID) : base(TYPE) {
        PlayerUUID = _playerUUID;
    }
}

[Serializable]
public class PlayerExited : BaseMessage {
    public const string TYPE = "PlayerExitedMessage";

    public string PlayerUUID;

    public PlayerExited(string _playerUUID) : base(TYPE) {
        PlayerUUID = _playerUUID;
    }
}

[Serializable]
public class PlayerNotExited : BaseMessage {
    public const string TYPE = "PlayerNotExitedMessage";

    public string PlayerUUID;

    public PlayerNotExited(string _playerUUID) : base(TYPE) {
        PlayerUUID = _playerUUID;
    }
}

[Serializable]
public class AvatarAnimationMessage : BaseMessage {
    public const string TYPE = "AvatarAnimationMessage";

    public string PlayerUUID;
    public SerializableDictionary ParameterDictionary;

    public AvatarAnimationMessage(string _playerUUID, SerializableDictionary _parameterDictionary) : base(TYPE) {
        PlayerUUID = _playerUUID;
        ParameterDictionary = _parameterDictionary;
    }
}

[Serializable]
public class AvatarSendHandSideMessage : BaseMessage {
    public const string TYPE = "AvatarSendHandSideMessage";

    public string PlayerUUID;
    public Side Side;

    public AvatarSendHandSideMessage(string _playerUUID, Side _side) : base(TYPE) {
        PlayerUUID = _playerUUID;
        Side = _side;
    }
}

[Serializable]
public class AvatarComponentEnablerMessage : BaseMessage {
    public const string TYPE = "AvatarComponentEnablerMessage";

    public string PlayerUUID;
    public AvatarComponentType ComponentType;
    public bool isActive;

    public AvatarComponentEnablerMessage(string _playerUUID, AvatarComponentType _componentType, bool _isActive) : base(TYPE) {
        PlayerUUID = _playerUUID;
        ComponentType = _componentType;
        isActive = _isActive;
    }
}

[Serializable]
public class AvatarTorchSFXMessage : BaseMessage {
    public const string TYPE = "AvatarTorchSFXMessage";

    public string PlayerUUID;
    public SerializableDictionary SFXDictionary;

    public AvatarTorchSFXMessage(string _playerUUID, SerializableDictionary _SFXDictionary) : base(TYPE) {
        PlayerUUID = _playerUUID;
        SFXDictionary = _SFXDictionary;
    }
}
