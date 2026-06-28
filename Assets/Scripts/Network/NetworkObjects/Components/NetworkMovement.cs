using Ubiq.Geometry;
using Ubiq.Messaging;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// In order make a NetworkObject working, it has to be child of the Ubiq Network GameObject.
/// </summary>
public class NetworkMovement : MonoBehaviour {
    public Transform Transform { get; set; }

    private NetworkContext context;
    private bool amIOwner;
    private bool amISender;
    private XRBaseInteractable interactable;

    void Awake() {
        context = NetworkScene.Register(this);

        TryGetComponent(out interactable);
        amIOwner = false;
        Transform = transform;
    }

    void FixedUpdate() {
        // Only if I am the sender I transmit the position
        if (amISender) {
            SendMovementMessage();
        }
    }

    public void SelectObject() {
        amIOwner = true;
        amISender = true;
        SendMovementMessage();
    }

    public void ReleaseObject() {
        amIOwner = false;
        SendMovementMessage();
    }

    public void DeselectObject() {
        amIOwner = false;
        amISender = false;
        SendMovementMessage();
    }

    private void SendMovementMessage() {
        if (Transform == null) return;

        MovementMessage message = new MovementMessage(
            Transforms.ToLocal(Transform, context.Scene.transform),
            amIOwner
        );

        context.SendJson(message);
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        MovementMessage message = _message.FromJson<MovementMessage>();

        Pose pose = Transforms.ToWorld(message.Pose, context.Scene.transform);
        Transform.SetPositionAndRotation(pose.position, pose.rotation);

        if (message.IsOwned) {
            // If object is taken by another, the current player is no longer the amISender
            amISender = false;
        }
    }

    void OnDestroy() {
        if (interactable) {
            interactable.selectEntered.RemoveAllListeners();
            interactable.selectExited.RemoveAllListeners();
        }
    }
}
