using System;
using System.Linq;
using System.Threading.Tasks;
using TinyJson;
using Ubiq.Messaging;
using Ubiq.Rooms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;



[System.Serializable]
public class BaseMessage {
    public string messageType; //Need to be public due to serialization

    public BaseMessage(String msgType) {
        this.messageType = msgType;
    }
}

[System.Serializable]
public class ReadyMessage : BaseMessage {
    public ReadyMessage() : base("ReadyMsg") { }
}

[System.Serializable]
public class LoadLevelCompletedMessage : BaseMessage {
    public LoadLevelCompletedMessage() : base("LoadLevelCompletedMsg") { }
}

[System.Serializable]
public class RecoverCurrentCounterRequestMessage : BaseMessage {
    public RecoverCurrentCounterRequestMessage() : base("RecoverCurrentCounterRequestMsg") { }
}

[System.Serializable]
public class RecoverCurrentCounterReplyMessage : BaseMessage {
    public int localCounter; //Need to be public due to serialization
    public RecoverCurrentCounterReplyMessage(int localCounter) : base("RecoverCurrentCounterReplyMsg") {
        this.localCounter = localCounter;
    }
}

public class MsgHandler : MonoBehaviour {
    public static MsgHandler Instance { get; private set; }
    public event EventHandler OnCounterRecoverFinished;
    public event EventHandler OnAllPeersLoadingLevelFinished;
    public event EventHandler<OnAllPeersReadyForChangeEventArgs> OnAllPeersReadyForChange;
    public class OnAllPeersReadyForChangeEventArgs : EventArgs {
        public String levelName;
    }

    [SerializeField]
    private RoomClient roomClient;
    private NetworkContext context;
    private bool wasCounterRequested;
    private int receiveReadyMsgCounter;
    private int receiveLoadCompleteMsgCounter;
    private int receiveRecoverCurrentCounterReplyCounter;
    private String currentRoom;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            return;
        }
    }

    private void Start() {
        context = NetworkScene.Register(this);
        receiveReadyMsgCounter = 0;
        receiveLoadCompleteMsgCounter = 0;
        receiveRecoverCurrentCounterReplyCounter = 0;
        currentRoom = "";
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
            context.SendJson<RecoverCurrentCounterReplyMessage>(new RecoverCurrentCounterReplyMessage(receiveReadyMsgCounter));
        } else {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); //Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
            RecoverCurrentCounterRequestMessageHandler();
        }

    }

    private void RecoverCurrentCounterReplyMessageHandler(RecoverCurrentCounterReplyMessage _msg) {
        if (wasCounterRequested && _msg.localCounter > receiveReadyMsgCounter) {
            receiveReadyMsgCounter = _msg.localCounter;
        }
    }

    private async void RecoverCurrentCounterProcessStatusChecker() {
        //Check whether the recovery counter process has ended and signal registered handler

        do {
            await Task.Delay(100);
        } while (receiveRecoverCurrentCounterReplyCounter < roomClient.Peers.Count());

        receiveRecoverCurrentCounterReplyCounter = 0;
        OnCounterRecoverFinished?.Invoke(this, EventArgs.Empty);
    }

    public async void SendReadyMessage() {
        if (context.Scene != null) {
            receiveReadyMsgCounter += 1;
            context.SendJson<ReadyMessage>(new ReadyMessage());
            if (receiveReadyMsgCounter == 1) {
                ChangeLevelHandler();
            }
        } else {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); //Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
            SendReadyMessage();
        }
    }

    public async void SendLoadLevelCompletedMessage() {
        if (context.Scene != null) {
            receiveLoadCompleteMsgCounter += 1;
            context.SendJson<LoadLevelCompletedMessage>(new LoadLevelCompletedMessage());
            if (receiveLoadCompleteMsgCounter == 1) {
                PeerLoadingHandler();
            }
        } else {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); //Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
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
        //New room join: reset everything;
        Debug.Log("New room joined: resetting counters");
        wasCounterRequested = !wasCounterRequested;
        receiveReadyMsgCounter = 0;
        receiveLoadCompleteMsgCounter = 0;
        receiveRecoverCurrentCounterReplyCounter = 0;

        await Task.Delay(1000);

        if (currentRoom == roomClient.Room.Name) {
            return;
        }

        currentRoom = roomClient.Room.Name;

        if (context.Scene != null && roomClient.Peers.Count() > 0) {
            context.SendJson<RecoverCurrentCounterRequestMessage>(
                new RecoverCurrentCounterRequestMessage());
        } else if (roomClient.Peers.Count() > 0) {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); //Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
            OnJoinedRoomHandler(_room);
        }
    }

    void OnDestroy() {
        roomClient.OnJoinedRoom.RemoveListener(OnJoinedRoomHandler);
    }

}