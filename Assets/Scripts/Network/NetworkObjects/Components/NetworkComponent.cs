using Ubiq.Messaging;
using UnityEngine;

/// <summary>
/// In order make a NetworkObject working, it has to be child of the Ubiq Network GameObject.
/// </summary>
public abstract class NetworkComponent : MonoBehaviour {
    protected NetworkContext Context { get; private set; }

    void Awake() {
        Context = NetworkScene.Register(this);
    }

    public abstract void ProcessMessage(ReferenceCountedSceneGraphMessage _message);
}
