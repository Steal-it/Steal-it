using TMPro;
using Ubiq.Rooms;
using UnityEngine;
using UnityEngine.UI;

public class RoomsListElement : MonoBehaviour {
    [SerializeField]
    private TMP_Text roomNameText;
    [SerializeField]
    private Button joinButton;

    private RoomClient roomClient;
    private string joinCode;

    void Awake() {
        joinButton.onClick.AddListener(Join);
    }

    private void Join() {
        if (!roomClient || string.IsNullOrEmpty(joinCode)) {
            return;
        }
        roomClient.Join(joincode: joinCode);
    }

    public void Bind(RoomClient _roomClient, IRoom _room) {
        roomClient = _roomClient;
        joinCode = _room.JoinCode;

        roomNameText.text = _room.Name;
    }

    void OnDestroy() {
        joinButton.onClick.RemoveAllListeners();
    }
}
