using System;
using System.Linq;
using System.Threading.Tasks;
using Ubiq.Messaging;
using Ubiq.Rooms;
using UnityEngine;
using WebSocketSharp;

public class MessageHandler : MonoBehaviour {
    public event EventHandler OnAllPeersReadyForChange;
    public event EventHandler OnClientAsServerChanged;

    public event EventHandler<OnApplySpectatorModeRequestEventArgs> OnApplySpectatorModeRequested;
    public class OnApplySpectatorModeRequestEventArgs : EventArgs {
        public string PlayerUUID;
    }
    public event EventHandler OnPlayerExited;

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

        receiveReadyMsgCounter = 0;
        wasCounterRequested = false;

        if (roomClient.Peers.Count() + 1 > 1) {
            // Cover the possibility of a player that press ready and another exit when only two people are in the room
            Debug.Log("All ready for change!");
            OnAllPeersReadyForChange?.Invoke(this, EventArgs.Empty);
        }
    }

    private async void PeerLoadingHandler() {
        do {
            await Task.Delay(100);
        } while (receiveLoadCompleteMsgCounter != roomClient.Peers.Count() + 1);

        Debug.Log("All ready for unlocking!");
        receiveLoadCompleteMsgCounter = 0;
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
        // TODO: OnCounterRecoverFinished?.Invoke(this, EventArgs.Empty);
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

    public async void SendNewClientAsServerElection() {
        if (roomClient.Peers.Count() == 0) return;

        if (context.Scene != null) {
            // Elect the first peer in the list as the one to act as the server
            IPeer peer = roomClient.Peers.First();
            context.SendJson(new NewClientAsServerElectionMessage(peer.uuid));
        } else {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); // Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
            SendNewClientAsServerElection();
        }
    }

    public async void SendActivateSpectatorModeMessage(string _playerUUID) {
        if (context.Scene != null) {
            context.SendJson(new ActivateSpectatorModeMessage(_playerUUID));
        } else {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); // Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
            SendActivateSpectatorModeMessage(_playerUUID);
        }
    }

    public async void SendPlayerExited() {
        if (context.Scene != null) {
            context.SendJson(new PlayerExited());
        } else {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); // Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
            SendPlayerExited();
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        BaseMessage message = _message.FromJson<BaseMessage>();
        switch (message.type) {
            case ReadyMessage.TYPE: {
                    Debug.Log($"Received {ReadyMessage.TYPE}");
                    receiveReadyMsgCounter += 1;
                    if (receiveReadyMsgCounter == 1) {
                        ChangeLevelHandler();
                    }
                }
                break;
            case LoadLevelCompletedMessage.TYPE: {
                    Debug.Log($"Received {LoadLevelCompletedMessage.TYPE}");
                    receiveLoadCompleteMsgCounter += 1;
                    if (receiveLoadCompleteMsgCounter == 1) {
                        PeerLoadingHandler();
                    }
                }
                break;
            case RecoverCurrentCounterRequestMessage.TYPE: {
                    Debug.Log($"Received {RecoverCurrentCounterRequestMessage.TYPE}");
                    wasCounterRequested = true;
                    RecoverCurrentCounterRequestMessageHandler();
                }
                break;
            case RecoverCurrentCounterReplyMessage.TYPE: {
                    Debug.Log($"Received {RecoverCurrentCounterReplyMessage.TYPE}");
                    RecoverCurrentCounterReplyMessage finalMessage = _message.FromJson<RecoverCurrentCounterReplyMessage>();
                    RecoverCurrentCounterReplyMessageHandler(finalMessage);
                    receiveRecoverCurrentCounterReplyCounter += 1;
                    if (receiveRecoverCurrentCounterReplyCounter == 1) {
                        RecoverCurrentCounterProcessStatusChecker();
                    }
                }
                break;
            case NewClientAsServerElectionMessage.TYPE: {
                    Debug.Log($"Received {NewClientAsServerElectionMessage.TYPE}");
                    NewClientAsServerElectionMessage finalMessage = _message.FromJson<NewClientAsServerElectionMessage>();
                    if (roomClient.Me.uuid == finalMessage.clientAsServerUuid) {
                        OnClientAsServerChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
                break;
            case ActivateSpectatorModeMessage.TYPE: {
                    ActivateSpectatorModeMessage msg = _message.FromJson<ActivateSpectatorModeMessage>();
                    OnApplySpectatorModeRequested?.Invoke(this, new OnApplySpectatorModeRequestEventArgs {
                        PlayerUUID = msg.playerUUID
                    });
                    Debug.Log($"Received {ActivateSpectatorModeMessage.TYPE}: {msg.playerUUID}");
                }
                break;
            case PlayerExited.TYPE: {
                    OnPlayerExited?.Invoke(this, EventArgs.Empty);
                    Debug.Log($"Received {PlayerExited.TYPE}");
                }
                break;
            default:
                Debug.LogWarning("Received unknown message! " + message.type);
                break;
        }
    }

    private async void OnJoinedRoomHandler(IRoom _room) {
        // Let Ubiq update Peers list
        await Task.Delay(100);

        if (roomClient.Peers.Count() == 0 && !_room.Name.IsNullOrEmpty()) {
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
