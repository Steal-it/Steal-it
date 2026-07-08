using System;

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

    public string playerUUID;

    public ActivateSpectatorModeMessage(string _playerUUID) : base(TYPE) {
        playerUUID = _playerUUID;
    }
}

[Serializable]
public class PlayerExited : BaseMessage {
    public const string TYPE = "PlayerExitedMessage";

    public PlayerExited() : base(TYPE) { }
}
