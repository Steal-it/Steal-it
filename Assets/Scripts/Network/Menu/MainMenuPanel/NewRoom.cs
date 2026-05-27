using UnityEngine;
using System;
using UnityEngine.UI;
using Ubiq.Rooms;

public class NewRoomButton : MonoBehaviour {
    // Expected to be called by a UI element
    [SerializeField]
    private Button button;

    private RoomClient roomClient;

    private void Awake() {
        button.onClick.AddListener(NewRoom);
    }

    private void Start() {
        roomClient = NetworkReferenceManager.Instance.RoomClient;
    }

    public void NewRoom() {
        string roomName = Guid.NewGuid().ToString().Substring(0, 7);
        roomClient.Join(
            name: roomName,
            publish: true
        );
    }

    private void OnDestroy() {
        button.onClick.RemoveListener(NewRoom);
    }
}