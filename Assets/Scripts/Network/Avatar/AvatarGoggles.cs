using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AvatarGoggles : AvatarComponentEnabler {
    void Start() {
        XRSocketInteractor gogglesSocket = FindFirstObjectByType<GogglesSocket>().GetComponent<XRSocketInteractor>();
        gogglesSocket.selectEntered.AddListener(EnableGoggles);
        if (!IsLocal) {
            NetworkObjectEnabler.OnMessageReceived += EnableOtherGoggles;
        }
    }

    private void EnableOtherGoggles(bool _active) {
        foreach (var gameObject in GameObjects) {
            gameObject.SetActive(_active);
        }
    }

    private void EnableGoggles(SelectEnterEventArgs _) {
        NetworkObjectEnabler.SendEnableParameters(true);
    }
}
