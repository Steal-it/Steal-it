using Ubiq.Avatars;
using Ubiq.Rooms;
using UnityEngine;

public class NetworkReferenceManager : MonoBehaviour {
    public static NetworkReferenceManager Instance { get; private set; }

    public RoomClient RoomClient => roomClient;
    public AvatarManager AvatarManager => avatarManager;
    public MsgHandler MsgHandler => msgHandler;
    public LocalLobbyMenu LocalLobbyMenu => localLobbyMenu;
    public RoomLobbyMenu RoomLobbyMenu => roomLobbyMenu;

    [SerializeField]
    private RoomClient roomClient;
    [SerializeField]
    private AvatarManager avatarManager;
    [SerializeField]
    private MsgHandler msgHandler;
    [SerializeField]
    private LocalLobbyMenu localLobbyMenu;
    [SerializeField]
    private RoomLobbyMenu roomLobbyMenu;

    void Awake() {
        if (Instance != null) {
            Debug.LogError($"An instance of {nameof(NetworkReferenceManager)} already exists!");
            return;
        }

        Instance = this;
    }
}
