using System;
using Ubiq.Dictionaries;
using Ubiq.Messaging;
using UnityEngine;

/// <summary>
/// In order make a NetworkObject working, it has to be child of the Ubiq Network GameObject.
/// </summary>
public class NetworkAnimation : MonoBehaviour
{
    public event EventHandler<OnMessageReceivedEventArgs> OnMessageReceived;
    public class OnMessageReceivedEventArgs : EventArgs
    {
        public SerializableDictionary ParameterDictionary;
    }

    private NetworkContext context;

    void Awake()
    {
        context = NetworkScene.Register(this);
    }

    public void SendAnimationParameters(SerializableDictionary _parameterDictionary)
    {
        AnimationMessage message = new AnimationMessage(
            _parameterDictionary
        );

        context.SendJson(message);
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage _message)
    {
        AnimationMessage message = _message.FromJson<AnimationMessage>();

        OnMessageReceived?.Invoke(this, new OnMessageReceivedEventArgs
        {
            ParameterDictionary = message.ParameterDictionary
        });
    }
}