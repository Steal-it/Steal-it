using System;
using Ubiq.Dictionaries;
using Ubiq.Messaging;
using UnityEngine;

/// <summary>
/// In order make a NetworkObject working, it has to be child of the Ubiq Network GameObject.
/// </summary>
public class NetworkAudio : NetworkComponent {
    public event EventHandler<OnMessageReceivedEventArgs> OnMessageReceived;
    public class OnMessageReceivedEventArgs : EventArgs {
        public SerializableDictionary SFXDictionary;
    }

    // private NetworkContext context;

    void Awake() {
        // context = NetworkScene.Register(this);
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
