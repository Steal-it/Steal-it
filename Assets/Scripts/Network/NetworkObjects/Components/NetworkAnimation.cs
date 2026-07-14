using System;
using Ubiq.Dictionaries;
using Ubiq.Messaging;

public class NetworkAnimation : NetworkComponent {
    public event EventHandler<OnMessageReceivedEventArgs> OnMessageReceived;
    public class OnMessageReceivedEventArgs : EventArgs {
        public SerializableDictionary ParameterDictionary;
    }

    void Start() {
        RegisterContext(this);
    }

    public void SendAnimationParameters(SerializableDictionary _parameterDictionary) {
        AnimationMessage message = new AnimationMessage(
            _parameterDictionary
        );

        Context.SendJson(message);
    }

    public override void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        AnimationMessage message = _message.FromJson<AnimationMessage>();

        OnMessageReceived?.Invoke(this, new OnMessageReceivedEventArgs {
            ParameterDictionary = message.ParameterDictionary
        });
    }
}
