using System;
using Ubiq.Messaging;
using UnityEngine;

public class NetworkHandSide : NetworkComponent {
    public event Action<Side> OnMessageReceived;
    void Awake() {
        RegisterContext(this);
    }

    public void SendSideParameters(Side _side) {
        HandSideMessage message = new HandSideMessage(
            _side
        );

        Context.SendJson(message);
    }

    public override void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        HandSideMessage message = _message.FromJson<HandSideMessage>();
        OnMessageReceived?.Invoke(message.side);
    }

}
