using System;
using Ubiq.Messaging;
using UnityEngine.Events;

public class NetworkObjectEnabler : NetworkComponent {
    public event Action<bool> OnMessageReceived;
    public UnityEvent<bool> OnActivationNeeded;

    void Start() {
        RegisterContext(this);
    }

    public void SendEnableParameters(bool _isActive) {
        EnabledMessage message = new EnabledMessage(
            _isActive
        );

        Context.SendJson(message);
    }

    public override void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        EnabledMessage message = _message.FromJson<EnabledMessage>();

        OnMessageReceived?.Invoke(message.isActive);
        OnActivationNeeded?.Invoke(message.isActive);
    }
}
