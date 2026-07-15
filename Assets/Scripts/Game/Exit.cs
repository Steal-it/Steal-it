using System;
using System.Collections.Generic;
using System.Linq;
using Ubiq.Rooms;
using Unity.XR.CoreUtils;
using UnityEngine;

public class Exit : MonoBehaviour {
    private MessageHandler messageHandler;
    private SpectatorModeManager spectatorModeManager;
    private RoomClient roomClient;
    private List<string> exitedPlayerList;
    private int deadPlayersCounter;

    void Start() {
        messageHandler = NetworkReferenceManager.Instance.MessageHandler;
        spectatorModeManager = NetworkReferenceManager.Instance.SpectatorModeManager;
        roomClient = NetworkReferenceManager.Instance.RoomClient;

        messageHandler.OnPlayerExited += MessageHandler_OnPlayerExited;
        messageHandler.OnPlayerNotExited += MessageHandler_OnPlayerNotExited;
        spectatorModeManager.OnSpectatorModeChanged += SpectatorModeManager_OnSpectatorModeChanged;

        exitedPlayerList = new List<string>();
    }

    void OnTriggerEnter(Collider _other) {
        if (_other.TryGetComponent<XROrigin>(out var _) && !spectatorModeManager.IsEnabled) {
            string localUUID = roomClient.Me.uuid;
            messageHandler.SendPlayerExited(localUUID);

            exitedPlayerList.Add(localUUID);
            CheckGameOver();
        }
    }

    void OnTriggerExit(Collider _other) {
        if (_other.TryGetComponent<XROrigin>(out var _) && !spectatorModeManager.IsEnabled) {
            string localUUID = roomClient.Me.uuid;
            messageHandler.SendPlayerNotExited(localUUID);

            exitedPlayerList.Remove(localUUID);
        }
    }

    private void MessageHandler_OnPlayerExited(object _sender, MessageHandler.OnPlayerExitedOrNotExitedEventArgs _event) {
        exitedPlayerList.Add(_event.PlayerUUID);
        CheckGameOver();
    }

    private void MessageHandler_OnPlayerNotExited(object _sender, MessageHandler.OnPlayerExitedOrNotExitedEventArgs _event) {
        exitedPlayerList.Remove(_event.PlayerUUID);
    }

    private void SpectatorModeManager_OnSpectatorModeChanged(object _sender, SpectatorModeManager.OnSpectatorModeChangeEventArgs _event) {
        deadPlayersCounter++;
        if (exitedPlayerList.Contains(_event.PlayerUUID)) {
            // The player was killed inside the final room
            exitedPlayerList.Remove(_event.PlayerUUID);
        }

        CheckGameOver();
    }

    private void CheckGameOver() {
        int playersCount = roomClient.Peers.Count() + 1;
        int exitedPlayersCount = exitedPlayerList.Count;

        if (exitedPlayersCount + deadPlayersCounter == playersCount) {
            // All alive players exited the maze
            if (spectatorModeManager.IsEnabled) {
                GameOver.Instance.Loser();
            } else {
                GameOver.Instance.Winner();
            }
        }
    }

    void OnDestroy() {
        messageHandler.OnPlayerExited -= MessageHandler_OnPlayerExited;
        messageHandler.OnPlayerNotExited -= MessageHandler_OnPlayerNotExited;
        spectatorModeManager.OnSpectatorModeChanged -= SpectatorModeManager_OnSpectatorModeChanged;
    }
}
