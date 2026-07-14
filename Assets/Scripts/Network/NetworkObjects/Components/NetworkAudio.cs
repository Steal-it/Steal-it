using System;
using Ubiq.Dictionaries;
using Ubiq.Messaging;

public class NetworkAudio : NetworkComponent {
    public event EventHandler<OnMessageReceivedEventArgs> OnMessageReceived;
    public class OnMessageReceivedEventArgs : EventArgs {
        public SerializableDictionary SFXDictionary;
    }

    void Start() {
        RegisterContext(this);
    }

    public void SendSFXs(SerializableDictionary _SFXDictionary) {
        AudioMessage message = new AudioMessage(
            _SFXDictionary
        );

        Context.SendJson(message);
    }

    public override void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        AudioMessage message = _message.FromJson<AudioMessage>();

        OnMessageReceived?.Invoke(this, new OnMessageReceivedEventArgs {
            SFXDictionary = message.SFXDictionary
        });
    }
}
