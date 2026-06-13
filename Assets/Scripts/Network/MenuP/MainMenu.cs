using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public event EventHandler OnNewRoomCreated;
    public event EventHandler OnRoomJoined;

    [SerializeField]
    private Button newRoomButton;
    [SerializeField]
    private Button joinRoomButton;
    [SerializeField]
    private GameObject roomsListPanel;

    void Awake() {
        newRoomButton.onClick.AddListener(CreateNewRoom);
        joinRoomButton.onClick.AddListener(OpenRoomsListPanel);

        roomsListPanel.SetActive(false);
    }

    private void CreateNewRoom() {
        // string roomName = Guid.NewGuid().ToString().Substring(0, 7);
        // NetworkReferenceManager.Instance.RoomClient.Join(
        //     name: roomName,
        //     publish: true
        // );

        OnNewRoomCreated?.Invoke(this, EventArgs.Empty);
    }

    private void OpenRoomsListPanel() {
        roomsListPanel.SetActive(true);

        // Even if the player did not actually join a room yet, we send the event so that
        // the observers (at least MonsterPack) know the desire of the player to join an existing room
        OnRoomJoined?.Invoke(this, EventArgs.Empty);
    }

    void OnDestroy() {
        newRoomButton.onClick.RemoveAllListeners();
        joinRoomButton.onClick.RemoveAllListeners();
    }
}