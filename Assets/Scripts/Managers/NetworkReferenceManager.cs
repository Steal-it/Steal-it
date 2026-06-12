using Ubiq.Avatars;
using Ubiq.Rooms;
using UnityEngine;

public class NetworkReferenceManager : MonoBehaviour {
    public static NetworkReferenceManager Instance { get; private set; }

    public RoomClient RoomClient => roomClient;
    public AvatarManager AvatarManager => avatarManager;
    public MsgHandler MsgHandler => msgHandler;
    public NewRoomButton NewRoomButton => newRoomButton;

    [SerializeField]
    private RoomClient roomClient;
    [SerializeField]
    private AvatarManager avatarManager;
    [SerializeField]
    private MsgHandler msgHandler;
    [SerializeField]
    private NewRoomButton newRoomButton;

    void Awake() {
        if (Instance != null) {
            Debug.LogError($"An instance of {nameof(NetworkReferenceManager)} already exists!");
            return;
        }

        Instance = this;
    }
}
