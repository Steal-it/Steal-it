using System;
using System.Collections.Generic;
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
    private Transform roomLobbySpawnPointCenter;
    [SerializeField]
    private Transform gameSpawnPointCenter;
    [SerializeField, Range(0.5f, 2)]
    private float spawnPointRadius = 1;

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
        (Vector3 position, Quaternion rotation) = GetSpawnPoint(false);
        rig.transform.SetPositionAndRotation(position, rotation);
        rigLocomotor.gameObject.SetActive(true);
    }

    private void LoadGame() {
        (Vector3 position, Quaternion rotation) = GetSpawnPoint(true);
        rig.transform.SetPositionAndRotation(position, rotation);
        rigLocomotor.gameObject.SetActive(true);

        OnGameLoaded?.Invoke(this, new OnGameLoadedEventArgs {
            IsClientAsServer = isClientAsServer
        });
    }

    private (Vector3 position, Quaternion rotation) GetSpawnPoint(bool _isGameSpwanPoint) {
        // Sort UUIDs so every client agrees on the same order
        RoomClient roomClient = NetworkReferenceManager.Instance.RoomClient;
        List<string> uuidSortedList = new List<string>();
        foreach (IPeer peer in roomClient.Peers) {
            uuidSortedList.Add(peer.uuid);
        }
        uuidSortedList = uuidSortedList.OrderBy(id => id).ToList();

        int index = uuidSortedList.IndexOf(roomClient.Me.uuid);
        Transform centerPoint = _isGameSpwanPoint ? gameSpawnPointCenter : roomLobbySpawnPointCenter;

        float angle = index * (360f / uuidSortedList.Count) * Mathf.Deg2Rad;
        Vector3 position = centerPoint.position + new Vector3(Mathf.Cos(angle) * spawnPointRadius, 0, Mathf.Sin(angle) * spawnPointRadius);
        Quaternion rotation = centerPoint.rotation;

        return (position, rotation);
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

    void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(roomLobbySpawnPointCenter.position, spawnPointRadius);
        Gizmos.DrawWireSphere(gameSpawnPointCenter.position, spawnPointRadius);
    }
}
