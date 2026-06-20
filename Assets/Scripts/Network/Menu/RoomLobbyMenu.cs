using System;
using UnityEngine;
using UnityEngine.UI;

public class RoomLobbyMenu : MonoBehaviour {
    public event EventHandler OnRoomExited;

    [SerializeField]
    private Button readyButton;
    [SerializeField]
    private Button exitButton;

    private MessageHandler messageHandler;
    private LevelManager levelManager;

    void Awake() {
        readyButton.onClick.AddListener(NotifyReady);
        exitButton.onClick.AddListener(ExitRoom);
    }

    void Start() {
        messageHandler = NetworkReferenceManager.Instance.MessageHandler;
        levelManager = NetworkReferenceManager.Instance.LevelManager;

        levelManager.OnMinPlayersNumberReached += LevelManager_OnMinPlayersNumberReached;
        levelManager.OnMinPlayersNumberLost += LevelManager_OnMinPlayersNumberLost;

        readyButton.interactable = false;
    }

    private void LevelManager_OnMinPlayersNumberReached(object sender, EventArgs e) {
        readyButton.interactable = true;
    }

    private void LevelManager_OnMinPlayersNumberLost(object sender, EventArgs e) {
        readyButton.interactable = false;
    }

    private void NotifyReady() {
        // Once the ready button is pressed, the player can only wait for the others to set ready and play
        readyButton.interactable = false;
        exitButton.interactable = false;

        messageHandler.SendReadyMessage();
    }

    private void ExitRoom() {
        if (NetworkReferenceManager.Instance.LevelManager.IsClientAsServer) {
            // The client who created the room exited, elect a new client that acts as the server
            messageHandler.SendNewClientAsServerElection();
        }

        NetworkReferenceManager.Instance.RoomClient.Join(
            name: null,
            publish: false
        );

        OnRoomExited?.Invoke(this, EventArgs.Empty);
    }

    void OnDestroy() {
        readyButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();

        levelManager.OnMinPlayersNumberReached -= LevelManager_OnMinPlayersNumberReached;
        levelManager.OnMinPlayersNumberLost -= LevelManager_OnMinPlayersNumberLost;
    }
}
