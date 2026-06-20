using System;
using UnityEngine;
using UnityEngine.UI;

public class LocalLobbyMenu : MonoBehaviour {
    public event EventHandler OnNewRoomCreated;

    [SerializeField]
    private Button newRoomButton;
    [SerializeField]
    private Button joinRoomButton;
    [SerializeField]
    private RoomsListPanel roomsListPanel;

    void Awake() {
        newRoomButton.onClick.AddListener(CreateNewRoom);
        joinRoomButton.onClick.AddListener(OpenRoomsListPanel);

        roomsListPanel.gameObject.SetActive(false);
    }

    private void CreateNewRoom() {
        string roomName = Guid.NewGuid().ToString().Substring(0, 7);
        NetworkReferenceManager.Instance.RoomClient.Join(
            name: roomName,
            publish: true
        );

        OnNewRoomCreated?.Invoke(this, EventArgs.Empty);
    }

    private void OpenRoomsListPanel() {
        roomsListPanel.gameObject.SetActive(true);
    }

    void OnDestroy() {
        newRoomButton.onClick.RemoveAllListeners();
        joinRoomButton.onClick.RemoveAllListeners();
    }
}