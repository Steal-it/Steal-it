using System;
using Ubiq.Messaging;

public class NetworkHandSide : NetworkComponent {
    public event Action<Side> OnMessageReceived;

    void Awake() {
        RegisterContext(this);
    }

    public void SendSideParameters(Side _side, string _uuid) {
        HandSideMessage message = new HandSideMessage(
            _side,
            _uuid
        );

        Context.SendJson(message);
    }

    public override void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        HandSideMessage message = _message.FromJson<HandSideMessage>();
        string playerUUID = gameObject.transform.parent.parent.gameObject.name;
        if (playerUUID != "Local Avatar") {
            playerUUID = playerUUID.Split('#')[1];
            if (playerUUID == message.uuid) {
                OnMessageReceived?.Invoke(message.side);
            }
        }
    }
}
