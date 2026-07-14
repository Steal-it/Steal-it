using System;
using Ubiq.Dictionaries;
using Ubiq.Messaging;

public class NetworkParticles : NetworkComponent {
    public event EventHandler<OnMessageReceivedEventArgs> OnMessageReceived;
    public class OnMessageReceivedEventArgs : EventArgs {
        public SerializableDictionary VFXDictionary;
    }

    void Start() {
        RegisterContext(this);
    }

    public void SendVFXs(SerializableDictionary _VFXDictionary) {
        AudioMessage message = new AudioMessage(
            _VFXDictionary
        );

        Context.SendJson(message);
    }

    public override void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        ParticlesMessage message = _message.FromJson<ParticlesMessage>();

        OnMessageReceived?.Invoke(this, new OnMessageReceivedEventArgs {
            VFXDictionary = message.VFXDictionary
        });
    }
}
