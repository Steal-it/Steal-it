using System;
using UnityEngine;
using UnityEngine.UI;

public class RoomLobbyMenu : MonoBehaviour {
    public event EventHandler OnRoomExited;

    [SerializeField]
    private Button readyButton;
    [SerializeField]
    private Button exitButton;

    private MsgHandler msgHandler;

    void Awake() {
        readyButton.onClick.AddListener(NotifyReady);
        exitButton.onClick.AddListener(ExitRoom);
    }

    void Start() {
        msgHandler = NetworkReferenceManager.Instance.MsgHandler;

        msgHandler.OnCounterRecoverFinished += MsgHandler_OnCounterRecoverFinished;
    }

    private void MsgHandler_OnCounterRecoverFinished(object _sender, EventArgs _event) {
        // readyButton.interactable = true;
    }

    private void NotifyReady() {
        msgHandler.SendReadyMessage();
    }

    private void ExitRoom() {
        if (NetworkReferenceManager.Instance.LevelManager.IsClientAsServer) {
            // The client who created the room exited, elect a new client that acts as the server
            msgHandler.SendNewClientAsServerElection();
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

        msgHandler.OnCounterRecoverFinished -= MsgHandler_OnCounterRecoverFinished;
    }
}
