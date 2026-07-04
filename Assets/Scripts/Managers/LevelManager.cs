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
    private MapConfigurationManager mapConfigurationManager;
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

    private void LoadScreen(object _sender, EventArgs _event) {
        mapConfigurationManager.ApplyRandomConfiguration();

        LoadGame();

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
        RoomClient roomClient = NetworkReferenceManager.Instance.RoomClient;
        Transform centerPoint = _isGameSpwanPoint ? gameSpawnPointCenter : roomLobbySpawnPointCenter;
        Quaternion rotation = centerPoint.rotation;

        int hash = roomClient.Me.uuid.GetHashCode();
        float angle = (hash & 0x7FFFFFFF) % 360 * Mathf.Deg2Rad;
        Vector3 candidatePosition = centerPoint.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnPointRadius;

        // Nudge if too close to an already-assigned point
        foreach (IPeer peer in roomClient.Peers) {
            int otherHash = peer.uuid.GetHashCode();
            float otherAngle = (otherHash & 0x7FFFFFFF) % 360 * Mathf.Deg2Rad;
            Vector3 otherPosition = centerPoint.position + new Vector3(Mathf.Cos(otherAngle), 0, Mathf.Sin(otherAngle)) * spawnPointRadius;

            if (Vector3.Distance(candidatePosition, otherPosition) < 0.5f) {
                // Nudge 45 degrees away
                angle += 45 * Mathf.Deg2Rad;
            }
            candidatePosition = centerPoint.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnPointRadius;
        }

        return (candidatePosition, rotation);
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
        if (roomLobbySpawnPointCenter != null) {
            Gizmos.DrawWireSphere(roomLobbySpawnPointCenter.position, spawnPointRadius);
        }
        if (gameSpawnPointCenter != null) {
            Gizmos.DrawWireSphere(gameSpawnPointCenter.position, spawnPointRadius);
        }
    }
}
