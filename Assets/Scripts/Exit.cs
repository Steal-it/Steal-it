using System;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;

public class Exit : MonoBehaviour {
    private MessageHandler messageHandler;
    private SpectatorModeManager spectatorModeManager;
    private int exitedPlayersCounter;
    private int deadPlayersCounter;

    void Start() {
        messageHandler = NetworkReferenceManager.Instance.MessageHandler;
        spectatorModeManager = NetworkReferenceManager.Instance.SpectatorModeManager;

        messageHandler.OnPlayerExited += MessageHandler_OnPlayerExited;
        spectatorModeManager.OnSpectatorModeChanged += SpectatorModeManager_OnSpectatorModeChanged;
    }

    void OnTriggerEnter(Collider _other) {
        if (_other.TryGetComponent<XROrigin>(out var _) && !spectatorModeManager.IsEnabled) {
            messageHandler.SendPlayerExited();

            exitedPlayersCounter++;
            CheckForAllPlayerExited();
        }
    }

    private void MessageHandler_OnPlayerExited(object _sender, EventArgs _event) {
        exitedPlayersCounter++;
        CheckForAllPlayerExited();
    }

    private void SpectatorModeManager_OnSpectatorModeChanged(object _sender, SpectatorModeManager.OnSpectatorModeChangeEventArgs _event) {
        deadPlayersCounter++;
        CheckForAllPlayerExited();
    }

    private void CheckForAllPlayerExited() {
        int playersCount = NetworkReferenceManager.Instance.RoomClient.Peers.Count() + 1;
        int alivePlayersCount = playersCount - deadPlayersCounter;
        if (exitedPlayersCounter == alivePlayersCount) {
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
        spectatorModeManager.OnSpectatorModeChanged -= SpectatorModeManager_OnSpectatorModeChanged;
    }
}
