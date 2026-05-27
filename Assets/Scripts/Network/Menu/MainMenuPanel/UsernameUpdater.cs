using UnityEngine;
using Ubiq.Rooms;
using Ubiq;
using TMPro;

public class UsernameUpdater : MonoBehaviour {
    private RoomClient roomClient;
    private TextMeshProUGUI text;

    void Start() {
        roomClient = NetworkReferenceManager.Instance.RoomClient;

        text = GetComponent<TextMeshProUGUI>();
        roomClient.OnPeerUpdated.AddListener(OnPeerListener);
    }

    private void OnPeerListener(IPeer peer) {
        if (roomClient.Me == peer) {
            updateUsername();
        }
    }

    private void updateUsername() {
        if (roomClient.Me != null) {
            string name = roomClient.Me[DisplayNameManager.KEY];
            if (!string.IsNullOrEmpty(name)) {
                text.text = $"Welcome {name}";
            }
        }
    }

    void OnDestroy() {
        roomClient.OnPeerUpdated.RemoveListener(OnPeerListener);
    }
}
