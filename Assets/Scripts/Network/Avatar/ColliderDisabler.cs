using Ubiq.Avatars;
using Ubiq.Rooms;
using UnityEngine;
using Avatar = Ubiq.Avatars.Avatar;

public class ColliderDisabler : MonoBehaviour {
    private RoomClient roomClient;
    private AvatarManager avatarManager;

    private void Start() {
        roomClient = NetworkReferenceManager.Instance.RoomClient;
        avatarManager = NetworkReferenceManager.Instance.AvatarManager;

        avatarManager.OnAvatarCreated.AddListener(AvatarCreatedHandler);
    }

    private void AvatarCreatedHandler(Avatar _avatar) {
        if (_avatar.Peer == roomClient.Me) {
            // Disable collider if the just spawn avatar is the local one
            TorsoIdentifier torso = _avatar.GetComponentInChildren<TorsoIdentifier>();
            if (torso) {
                Collider col = torso.GetComponent<Collider>();
                col.enabled = false;
            }
        }
    }

    void OnDestroy() {
        avatarManager.OnAvatarCreated.RemoveListener(AvatarCreatedHandler);
    }
}
