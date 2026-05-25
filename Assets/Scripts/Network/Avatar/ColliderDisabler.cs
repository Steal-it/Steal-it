using System;
using Ubiq.Messaging;
using Ubiq.Rooms;
using Ubiq.Spawning;
using UnityEngine;

namespace Ubiq.Avatars
{
    public class ColliderDisabler : MonoBehaviour
    {
        public AvatarManager avatarManager;

        [SerializeField]
        private RoomClient roomClient;

        private void Start()
        {
            avatarManager.OnAvatarCreated.AddListener(avatarCreatedHandler);
        }

        private void avatarCreatedHandler(Avatar avatar) {
            if(avatar.Peer == roomClient.Me)
            {
                //Disable collider if the just spawn avatar is the local one
                //avatar.gameObject.
                var torso = avatar.GetComponentInChildren<TorsoIdentifier>();
                if(torso)
                {
                    var col = torso.GetComponent<Collider>();
                    col.enabled = false;
                }
            }
        }
    }
}