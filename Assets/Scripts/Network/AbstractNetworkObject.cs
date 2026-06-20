using System.Collections.Generic;
using Ubiq.Geometry;
using Ubiq.Messaging;
using UnityEngine;

/// <summary>
/// In order make a NetworkObject working, it has to be child of the Ubiq Network GameObject
/// </summary>
public abstract class AbstractNetworkObject : MonoBehaviour {
    protected NetworkContext Context { private get; set; }
    protected bool AmIOwner { private get; set; }
    protected bool AmISender { private get; set; }
    protected Transform Transform { get; set; }
    protected AbstractNetworkAnimator NetworkAnimator { get; set; }

    protected void OnAwake(AbstractNetworkObject _this) {
        Context = NetworkScene.Register(_this);

        AmIOwner = false;
        Transform = transform;
    }

    protected void OnFixedUpdate() {
        // Only if I am the sender I transmit the position
        if (AmISender) {
            SendOnFixedUpdate();
            SendMovementMessage();
        } else {
            NotSendOnFixedUpdate();
        }
    }

    protected void SelectObject() {
        AmIOwner = true;
        AmISender = true;
        SendMovementMessage();
    }

    protected void ReleaseObject() {
        AmIOwner = false;
        SendMovementMessage();
    }

    protected void DeselectObject() {
        AmIOwner = false;
        AmISender = false;
        SendMovementMessage();
    }

    private void SendMovementMessage() {
        MovementMessage message = new MovementMessage(
            Transforms.ToLocal(Transform, Context.Scene.transform),
            AmIOwner
        );

        SendMessage(message);
    }

    // protected void SendAnimationParameters(Dictionary<string, AnimationMessage.IAnimatorParameter> _parameterDictionary) {
    protected void SendAnimationParameters(Dictionary<string, bool> _parameterDictionary) {
        AnimationMessage message = new AnimationMessage();
        foreach (var entry in _parameterDictionary) {
            message.ParameterDictionary.Add(entry.Key, entry.Value);
        }

        print("send: " + message.ParameterDictionary.Count);

        SendMessage(message);
    }

    private void SendMessage(BaseNetworkObjectMessage _message) {
        Context.SendJson(_message);
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        BaseNetworkObjectMessage message = _message.FromJson<BaseNetworkObjectMessage>();

        switch (message.Type) {
            case MovementMessage.TYPE: {
                    MovementMessage castedMessage = _message.FromJson<MovementMessage>();

                    Pose pose = Transforms.ToWorld(castedMessage.Pose, Context.Scene.transform);
                    Transform.SetPositionAndRotation(pose.position, pose.rotation);

                    if (castedMessage.IsOwned) {
                        // If object is taken by another, the current player is no longer the amISender
                        AmISender = false;
                        NotOwnedOnReceived();
                    } else {
                        OwnedOnReceived();
                    }
                }
                break;
            case AnimationMessage.TYPE: {
                    if (NetworkAnimator == null) {
                        Debug.LogWarning(AnimationMessage.TYPE + " received but NetworkAnimator is null!");
                        return;
                    }

                    AnimationMessage castedMessage = _message.FromJson<AnimationMessage>();

                    print("receive: " + castedMessage.ParameterDictionary.Count);

                    // TODO: ParameterDictionary probably null or something like that
                    NetworkAnimator.SetParameterDictionary(castedMessage.ParameterDictionary);
                }
                break;
            default:
                Debug.LogWarning("Received unknown message! " + message.Type);
                break;
        }
    }

    protected abstract void SendOnFixedUpdate();

    protected abstract void NotSendOnFixedUpdate();

    protected abstract void OwnedOnReceived();

    protected abstract void NotOwnedOnReceived();
}
