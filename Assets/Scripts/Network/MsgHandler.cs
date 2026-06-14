using System;
using System.Linq;
using System.Threading.Tasks;
using Ubiq.Messaging;
using Ubiq.Rooms;
using UnityEngine;
using WebSocketSharp;

#region Messages
[Serializable]
public class BaseMessage {
    public string messageType;

    public BaseMessage(string msgType) {
        messageType = msgType;
    }
}

[Serializable]
public class ReadyMessage : BaseMessage {
    public ReadyMessage() : base("ReadyMsg") { }
}

[Serializable]
public class LoadLevelCompletedMessage : BaseMessage {
    public LoadLevelCompletedMessage() : base("LoadLevelCompletedMsg") { }
}

[Serializable]
public class RecoverCurrentCounterRequestMessage : BaseMessage {
    public RecoverCurrentCounterRequestMessage() : base("RecoverCurrentCounterRequestMsg") { }
}

[Serializable]
public class RecoverCurrentCounterReplyMessage : BaseMessage {
    public int localCounter;
    public RecoverCurrentCounterReplyMessage(int localCounter) : base("RecoverCurrentCounterReplyMsg") {
        this.localCounter = localCounter;
    }
}
#endregion

public class MsgHandler : MonoBehaviour {
    public event EventHandler OnCounterRecoverFinished;
    public event EventHandler OnAllPeersLoadingLevelFinished;
    public event EventHandler<OnAllPeersReadyForChangeEventArgs> OnAllPeersReadyForChange;
    public class OnAllPeersReadyForChangeEventArgs : EventArgs {
        public string levelName;
    }

    private RoomClient roomClient;
    private NetworkContext context;
    private bool wasCounterRequested;
    private int receiveReadyMsgCounter;
    private int receiveLoadCompleteMsgCounter;
    private int receiveRecoverCurrentCounterReplyCounter;

    private void Start() {
        roomClient = NetworkReferenceManager.Instance.RoomClient;

        context = NetworkScene.Register(this);
        receiveReadyMsgCounter = 0;
        receiveLoadCompleteMsgCounter = 0;
        receiveRecoverCurrentCounterReplyCounter = 0;
        roomClient.OnJoinedRoom.AddListener(OnJoinedRoomHandler);
    }

    private async void ChangeLevelHandler() {
        do {
            await Task.Delay(100);
        } while (receiveReadyMsgCounter != roomClient.Peers.Count() + 1);

        Debug.Log("All ready for change!");
        receiveReadyMsgCounter = 0;
        wasCounterRequested = false;

        OnAllPeersReadyForChange?.Invoke(this, new OnAllPeersReadyForChangeEventArgs { levelName = "Test" });
    }

    private async void PeerLoadingHandler() {
        do {
            await Task.Delay(100);
        } while (receiveLoadCompleteMsgCounter != roomClient.Peers.Count() + 1);

        Debug.Log("All ready for unlocking!");
        receiveLoadCompleteMsgCounter = 0;
        OnAllPeersLoadingLevelFinished?.Invoke(this, EventArgs.Empty);
    }

    private async void RecoverCurrentCounterRequestMessageHandler() {
        if (context.Scene != null) {
            context.SendJson(new RecoverCurrentCounterReplyMessage(receiveReadyMsgCounter));
        } else {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); // Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
            RecoverCurrentCounterRequestMessageHandler();
        }

    }

    private void RecoverCurrentCounterReplyMessageHandler(RecoverCurrentCounterReplyMessage _msg) {
        if (wasCounterRequested && _msg.localCounter > receiveReadyMsgCounter) {
            receiveReadyMsgCounter = _msg.localCounter;
        }
    }

    private async void RecoverCurrentCounterProcessStatusChecker() {
        // Check whether the recovery counter process has ended and signal registered handler
        do {
            await Task.Delay(100);
        } while (receiveRecoverCurrentCounterReplyCounter == roomClient.Peers.Count());

        receiveRecoverCurrentCounterReplyCounter = 0;
        OnCounterRecoverFinished?.Invoke(this, EventArgs.Empty);
    }

    public async void SendReadyMessage() {
        if (context.Scene != null) {
            receiveReadyMsgCounter += 1;
            context.SendJson(new ReadyMessage());
            if (receiveReadyMsgCounter == 1) {
                ChangeLevelHandler();
            }
        } else {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); // Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
            SendReadyMessage();
        }
    }

    public async void SendLoadLevelCompletedMessage() {
        if (context.Scene != null) {
            receiveLoadCompleteMsgCounter += 1;
            context.SendJson(new LoadLevelCompletedMessage());
            if (receiveLoadCompleteMsgCounter == 1) {
                PeerLoadingHandler();
            }
        } else {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); // Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
            SendLoadLevelCompletedMessage();
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage _msg) {
        BaseMessage message = _msg.FromJson<BaseMessage>();
        switch (message.messageType) {
            case "ReadyMsg":
                Debug.Log("Received Ready Msg");
                receiveReadyMsgCounter += 1;
                if (receiveReadyMsgCounter == 1) {
                    ChangeLevelHandler();
                }
                break;
            case "LoadLevelCompletedMsg":
                Debug.Log("Received Load Msg");
                receiveLoadCompleteMsgCounter += 1;
                if (receiveLoadCompleteMsgCounter == 1) {
                    PeerLoadingHandler();
                }
                break;
            case "RecoverCurrentCounterRequestMsg":
                wasCounterRequested = true;
                Debug.Log("Received counter request");
                RecoverCurrentCounterRequestMessageHandler();
                break;
            case "RecoverCurrentCounterReplyMsg":
                Debug.Log("Received counter reply");
                RecoverCurrentCounterReplyMessage finalMessage = _msg.FromJson<RecoverCurrentCounterReplyMessage>();
                RecoverCurrentCounterReplyMessageHandler(finalMessage);
                receiveRecoverCurrentCounterReplyCounter += 1;
                if (receiveRecoverCurrentCounterReplyCounter == 1) {
                    RecoverCurrentCounterProcessStatusChecker();
                }
                break;
            default:
                Debug.LogError("Received unknown message!" + message.messageType);
                break;
        }
    }

    private async void OnJoinedRoomHandler(IRoom _room) {
        if (roomClient.Peers.Count() == 0 && !_room.Name.IsNullOrEmpty()) {
            OnCounterRecoverFinished?.Invoke(this, EventArgs.Empty);
            return;
        }
        if (context.Scene != null) {
            context.SendJson(new RecoverCurrentCounterRequestMessage());
        } else {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); // Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
            OnJoinedRoomHandler(_room);
        }
    }

    void OnDestroy() {
        roomClient.OnJoinedRoom.RemoveListener(OnJoinedRoomHandler);
    }
}
