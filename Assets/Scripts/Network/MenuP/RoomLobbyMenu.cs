using System;
using UnityEngine;
using UnityEngine.UI;

public class RoomLobbyMenu : MonoBehaviour {
    public event EventHandler OnRoomExited;

    [SerializeField]
    private Button readyButton;
    [SerializeField]
    private Button exitButton;

    void Awake() {
        readyButton.onClick.AddListener(NotifyReady);
        exitButton.onClick.AddListener(ExitRoom);
    }

    private void NotifyReady() {

    }

    private void ExitRoom() {
        NetworkReferenceManager.Instance.RoomClient.Join(
            name: null,
            publish: false
        );

        OnRoomExited?.Invoke(this, EventArgs.Empty);
    }

    void OnDestroy() {
        readyButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }
}
