using System;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RoomLobby : MonoBehaviour {
    public event EventHandler OnRoomExited;

    private MessageHandler messageHandler;
    private LevelManager levelManager;

    [SerializeField]
    private XRSimpleInteractable readyButton;
    [SerializeField]
    private TMP_Text readyText;
    [SerializeField]
    private XRSimpleInteractable exitButton;
    [SerializeField]
    private TMP_Text exitText;

    void Start() {
        messageHandler = NetworkReferenceManager.Instance.MessageHandler;
        levelManager = NetworkReferenceManager.Instance.LevelManager;

        readyButton.selectEntered.AddListener(OnReadyPressed);
        exitButton.selectEntered.AddListener(OnExitPressed);

        levelManager.OnMinPlayersNumberReached += LevelManager_OnMinPlayersNumberReached;
        levelManager.OnMinPlayersNumberLost += LevelManager_OnMinPlayersNumberLost;

        DeactivateReadyButton();
    }

    private void OnReadyPressed(SelectEnterEventArgs _event) {
        // Once the ready button is pressed, the player can only wait for the others to set ready and play
        DeactivateReadyButton();
        DeactivateExitButton();

        messageHandler.SendReadyMessage();
    }

    private void OnExitPressed(SelectEnterEventArgs _event) {
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

    private void LevelManager_OnMinPlayersNumberReached(object _sender, EventArgs _event) {
        ActivateReadyButton();
    }

    private void LevelManager_OnMinPlayersNumberLost(object _sender, EventArgs _event) {
        DeactivateReadyButton();
    }

    private void ActivateReadyButton() {
        readyButton.selectEntered.AddListener(OnReadyPressed);
        readyText.color = Color.white;
    }

    private void DeactivateReadyButton() {
        readyButton.selectEntered.RemoveListener(OnReadyPressed);
        readyText.color = Color.gray;
    }

    private void ActivateExitButton() {
        exitButton.selectEntered.AddListener(OnExitPressed);
        exitText.color = Color.white;
    }

    private void DeactivateExitButton() {
        exitButton.selectEntered.RemoveListener(OnExitPressed);
        exitText.color = Color.gray;
    }

    void OnDestroy() {
        readyButton.selectEntered.RemoveListener(OnReadyPressed);
        exitButton.selectEntered.RemoveListener(OnExitPressed);

        levelManager.OnMinPlayersNumberReached -= LevelManager_OnMinPlayersNumberReached;
        levelManager.OnMinPlayersNumberLost -= LevelManager_OnMinPlayersNumberLost;
    }
}
