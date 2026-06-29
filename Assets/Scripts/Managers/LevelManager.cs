using System;
using System.Linq;
using Ubiq.Rooms;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

public class LevelManager : MonoBehaviour {
    public event EventHandler<OnGameLoadedEventArgs> OnGameLoaded;
    public class OnGameLoadedEventArgs : EventArgs {
        public bool IsClientAsServer;
    }
    public event EventHandler OnMinPlayersNumberReached;
    public event EventHandler OnMinPlayersNumberLost;

    public bool IsClientAsServer => isClientAsServer;

    [SerializeField]
    private XROrigin rig;
    [SerializeField]
    private LocomotionMediator rigLocomotor;
    [SerializeField]
    private Transform localLobbySpawnPoint;
    [SerializeField]
    private Transform roomLobbySpawnPoint;
    [SerializeField]
    private Transform gameSpawnPoint;

    private RoomClient roomClient;
    private MessageHandler messageHandler;
    private LocalLobbyMenu localLobbyMenu;
    private RoomsListPanel roomsListPanel;
    private RoomLobbyMenu roomLobbyMenu;
    private bool isClientAsServer;
    private int minPlayersNumber = 2;
    private bool isMinPlayerNumberReached;

    void Start() {
        roomClient = NetworkReferenceManager.Instance.RoomClient;
        messageHandler = NetworkReferenceManager.Instance.MessageHandler;
        localLobbyMenu = NetworkReferenceManager.Instance.LocalLobbyMenu;
        roomsListPanel = NetworkReferenceManager.Instance.RoomsListPanel;
        roomLobbyMenu = NetworkReferenceManager.Instance.RoomLobbyMenu;

        roomClient.OnPeerAdded.AddListener(RoomClient_OnPeerAdded);
        roomClient.OnPeerRemoved.AddListener(RoomClient_OnPeerRemoved);
        messageHandler.OnAllPeersReadyForChange += LoadScreen;
        messageHandler.OnClientAsServerChanged += MessageHandler_OnClientAsServerChanged;
        localLobbyMenu.OnNewRoomCreated += MainMenu_OnNewRoomCreated;
        roomsListPanel.OnRoomJoined += RoomsListPanel_OnRoomJoined;
        roomLobbyMenu.OnRoomExited += RoomLobbyMenu_OnRoomExited;

        LoadLocalLobby();
    }

    private void RoomClient_OnPeerAdded(IPeer _peer) {
        if (!isMinPlayerNumberReached && roomClient.Peers.Count() + 1 == minPlayersNumber) {
            isMinPlayerNumberReached = true;
            OnMinPlayersNumberReached?.Invoke(this, EventArgs.Empty);
        }
    }

    private void RoomClient_OnPeerRemoved(IPeer _peer) {
        if (isMinPlayerNumberReached && roomClient.Peers.Count() + 1 == minPlayersNumber - 1) {
            isMinPlayerNumberReached = false;
            OnMinPlayersNumberLost?.Invoke(this, EventArgs.Empty);
        }
    }

    private void LoadScreen(object _sender, MessageHandler.OnAllPeersReadyForChangeEventArgs _event) {
        switch (_event.levelName) {
            // TODO: huh?
            case "Test":
                LoadGame();
                break;
            default:
                Debug.LogError("Attempt to switch to " + _event.levelName + " but it does not exists");
                return;
        }

        messageHandler.SendLoadLevelCompletedMessage();
    }

    private void MessageHandler_OnClientAsServerChanged(object _sender, EventArgs _event) {
        isClientAsServer = true;
    }

    private void MainMenu_OnNewRoomCreated(object _sender, EventArgs _event) {
        LoadRoomLobby();
        isClientAsServer = true;
    }

    private void RoomsListPanel_OnRoomJoined(object _sender, EventArgs _event) {
        LoadRoomLobby();
        isClientAsServer = false;
    }

    private void RoomLobbyMenu_OnRoomExited(object _sender, EventArgs _event) {
        LoadLocalLobby();
        isClientAsServer = false;
    }

    private void LoadLocalLobby() {
        rig.transform.SetPositionAndRotation(localLobbySpawnPoint.position, localLobbySpawnPoint.rotation);
        rigLocomotor.gameObject.SetActive(false);
    }

    private void LoadRoomLobby() {
        rig.transform.SetPositionAndRotation(roomLobbySpawnPoint.position, roomLobbySpawnPoint.rotation);
        rigLocomotor.gameObject.SetActive(true);
    }

    private void LoadGame() {
        rig.transform.SetPositionAndRotation(gameSpawnPoint.position, gameSpawnPoint.rotation);
        rigLocomotor.gameObject.SetActive(true);

        OnGameLoaded?.Invoke(this, new OnGameLoadedEventArgs {
            IsClientAsServer = isClientAsServer
        });
    }

    void OnDestroy() {
        roomClient.OnPeerAdded.RemoveListener(RoomClient_OnPeerAdded);
        roomClient.OnPeerRemoved.RemoveListener(RoomClient_OnPeerRemoved);
        messageHandler.OnAllPeersReadyForChange -= LoadScreen;
        messageHandler.OnClientAsServerChanged -= MessageHandler_OnClientAsServerChanged;
        localLobbyMenu.OnNewRoomCreated -= MainMenu_OnNewRoomCreated;
        roomsListPanel.OnRoomJoined -= RoomsListPanel_OnRoomJoined;
        roomLobbyMenu.OnRoomExited -= RoomLobbyMenu_OnRoomExited;
    }
}
