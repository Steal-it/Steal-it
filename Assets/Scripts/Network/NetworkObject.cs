using Ubiq.Geometry;
using Ubiq.Messaging;
using UnityEngine;

public abstract class NetworkObject : MonoBehaviour {
    protected NetworkContext Context { private get; set; }
    protected bool AmIOwner { private get; set; }
    protected bool AmISender { private get; set; }

    protected void OnAwake() {
        AmIOwner = false;

        Context = NetworkScene.Register(this);
    }

    protected void OnFixedUpdate() {
        // Only if I am the sender I transmit the position
        if (AmISender) {
            SendOnFixedUpdate();
            SendMessage();
        } else {
            NotSendOnFixedUpdate();
        }
    }

    protected void SelectObject() {
        AmIOwner = true;
        AmISender = true;
        SendMessage();
    }

    protected void ReleaseObject() {
        AmIOwner = false;
        SendMessage();
    }

    protected void DeselectObject() {
        AmIOwner = false;
        AmISender = false;
        SendMessage();
    }

    protected void SendMessage() {
        MovementMessage message = new MovementMessage {
            Position = Transforms.ToLocal(transform, Context.Scene.transform),
            IsOwned = AmIOwner
        };

        Context.SendJson(message);
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        MovementMessage message = _message.FromJson<MovementMessage>();
        Pose pose = Transforms.ToWorld(message.Position, Context.Scene.transform);
        transform.SetPositionAndRotation(pose.position, pose.rotation);

        if (message.IsOwned) {
            // If object is taken by another, the current player is no longer the amISender
            AmISender = false;
            NotOwnedOnReceived();
        } else {
            OwnedOnReceived();
        }
    }

    protected abstract void SendOnFixedUpdate();

    protected abstract void NotSendOnFixedUpdate();

    protected abstract void OwnedOnReceived();

    protected abstract void NotOwnedOnReceived();
}
