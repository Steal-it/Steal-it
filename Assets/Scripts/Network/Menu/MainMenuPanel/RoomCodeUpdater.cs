using Ubiq.Rooms;
using UnityEngine;
using TMPro;

public class RoomCodeUpdater : MonoBehaviour {
    private RoomClient roomClient;
    private TextMeshProUGUI text;

    void Start() {
        roomClient = NetworkReferenceManager.Instance.RoomClient;

        text = GetComponent<TextMeshProUGUI>();
        roomClient.OnJoinedRoom.AddListener(OnJoinRoomListener);
    }

    private void OnJoinRoomListener(IRoom room) {
        if (room != null && room.UUID != null && room.UUID.Length > 0) {
            text.text = $"Room name: {room.Name}";
        }
    }

    void OnDestroy() {
        roomClient.OnJoinedRoom.RemoveListener(OnJoinRoomListener);
    }
}
