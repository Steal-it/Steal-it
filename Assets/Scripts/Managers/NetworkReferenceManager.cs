using Ubiq.Avatars;
using Ubiq.Rooms;
using UnityEngine;

public class NetworkReferenceManager : MonoBehaviour {
    public static NetworkReferenceManager Instance { get; private set; }

    public RoomClient RoomClient => roomClient;
    public AvatarManager AvatarManager => avatarManager;
    public SpectatorModeManager SpectatorModeManager => spectatorModeManager;
    public MessageHandler MessageHandler => messageHandler;
    public LevelManager LevelManager => levelManager;
    public LocalLobbyMenu LocalLobbyMenu => localLobbyMenu;
    public RoomsListPanel RoomsListPanel => roomsListPanel;
    public RoomLobbyMenu RoomLobbyMenu => roomLobbyMenu;

    [SerializeField]
    private RoomClient roomClient;
    [SerializeField]
    private AvatarManager avatarManager;
    [SerializeField]
    private SpectatorModeManager spectatorModeManager;
    [SerializeField]
    private MessageHandler messageHandler;
    [SerializeField]
    private LevelManager levelManager;
    [SerializeField]
    private LocalLobbyMenu localLobbyMenu;
    [SerializeField]
    private RoomsListPanel roomsListPanel;
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
