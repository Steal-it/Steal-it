using System;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;

public class Exit : MonoBehaviour {
    private MessageHandler messageHandler;
    private SpectatorModeManager spectatorModeManager;
    private int exitedPlayersCounter;

    void Start() {
        messageHandler = NetworkReferenceManager.Instance.MessageHandler;
        spectatorModeManager = NetworkReferenceManager.Instance.SpectatorModeManager;

        messageHandler.OnPlayerExited += MessageHandler_OnPlayerExited;
    }

    void OnTriggerEnter(Collider _other) {
        if (_other.TryGetComponent<XROrigin>(out var _) && !spectatorModeManager.IsEnabled) {
            messageHandler.SendPlayerExited();

            UpdateCounter();
        }
    }

    private void MessageHandler_OnPlayerExited(object _sender, EventArgs _event) {
        UpdateCounter();
    }

    private void UpdateCounter() {
        exitedPlayersCounter++;

        int playersCount = NetworkReferenceManager.Instance.RoomClient.Peers.Count() + 1;
        int alivePlayersCount = playersCount - spectatorModeManager.DeadPlayersCounter;
        if (exitedPlayersCounter == alivePlayersCount) {
            Debug.Log("QUIT");

            // All alive players exited the maze
            Application.Quit();
        }
    }

    void OnDestroy() {
        messageHandler.OnPlayerExited -= MessageHandler_OnPlayerExited;
    }
}
