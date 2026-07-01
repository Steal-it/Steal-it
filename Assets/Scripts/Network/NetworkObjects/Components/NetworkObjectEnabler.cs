using System;
using Ubiq.Messaging;
using UnityEngine;

public class NetworkObjectEnabler : MonoBehaviour {
    public event Action<bool> OnMessageReceived;
    private NetworkContext context;

    void Awake() {
        context = NetworkScene.Register(this);
    }

    public void SendEnableParameters(bool _isActive) {
        EnabledMessage message = new EnabledMessage(
            _isActive
        );

        context.SendJson(message);
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        EnabledMessage message = _message.FromJson<EnabledMessage>();

        OnMessageReceived?.Invoke(message.isActive);
    }
}
