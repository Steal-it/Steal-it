using Ubiq.Rooms;
using UnityEngine;

namespace Ubiq.Avatars {
    public class ColliderDisabler : MonoBehaviour {
        private RoomClient roomClient;
        private AvatarManager avatarManager;

        private void Start() {
            roomClient = NetworkReferenceManager.Instance.RoomClient;
            avatarManager = NetworkReferenceManager.Instance.AvatarManager;

            avatarManager.OnAvatarCreated.AddListener(avatarCreatedHandler);
        }

        private void avatarCreatedHandler(Avatar _avatar) {
            if (_avatar.Peer == roomClient.Me) {
                //Disable collider if the just spawn avatar is the local one
                //avatar.gameObject.
                var torso = _avatar.GetComponentInChildren<TorsoIdentifier>();
                if (torso) {
                    var col = torso.GetComponent<Collider>();
                    col.enabled = false;
                }
            }
        }
    }
}