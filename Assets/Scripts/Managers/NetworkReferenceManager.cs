using Ubiq.Avatars;
using Ubiq.Rooms;
using UnityEngine;

public class NetworkReferenceManager : MonoBehaviour {
    public static NetworkReferenceManager Instance { get; private set; }

    public RoomClient RoomClient => roomClient;
    public AvatarManager AvatarManager => avatarManager;


    [SerializeField]
    private RoomClient roomClient;
    [SerializeField]
    private AvatarManager avatarManager;

    void Awake() {
        if (Instance != null) {
            Debug.LogError($"An instance of {nameof(NetworkReferenceManager)} already exists!");
            return;
        }

        Instance = this;
    }
}
