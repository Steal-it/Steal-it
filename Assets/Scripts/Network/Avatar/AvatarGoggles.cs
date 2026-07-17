using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AvatarGoggles : AvatarComponentEnabler {
    private Transform parent;

    void Start() {
        XRSocketInteractor gogglesSocket = FindFirstObjectByType<GogglesSocket>().GetComponent<XRSocketInteractor>();
        gogglesSocket.selectEntered.AddListener(EnableGoggles);
        if (!IsLocal) {
            NetworkReferenceManager.Instance.MessageHandler.OnAvatarComponentEnablerMessageReceived += OnAvatarComponentEnablerMessageReceived;
        }
    }

    private void OnAvatarComponentEnablerMessageReceived(object _sender, MessageHandler.OnAvatarComponentEnablerMessageReceivedEventArgs _args) {
        if (_args.ComponentType != AvatarComponentType.Goggles) {
            return;
        }

        if (parent == null) {
            parent = transform;
        }

        string playerUUID = parent.name;

        while (!playerUUID.Contains("Remote Avatar") && !playerUUID.Contains("Local Avatar")) {
            parent = parent.parent;
            playerUUID = parent.name;
        }

        if (playerUUID != "Local Avatar") {
            playerUUID = playerUUID.Split('#')[1];
            if (playerUUID == _args.PlayerUUID) {
                EnableOtherGoggles(_args.IsActive);
            }
        }

    }

    private void EnableOtherGoggles(bool _active) {
        foreach (var gameObject in GameObjects) {
            gameObject.SetActive(_active);
        }
    }

    private void EnableGoggles(SelectEnterEventArgs _) {
        NetworkReferenceManager.Instance.MessageHandler.SendAvatarComponentEnablerMessage(AvatarComponentType.Goggles, true);
    }
}
