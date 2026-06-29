using Ubiq.Geometry;
using Ubiq.Messaging;
using UnityEngine;

public class NetworkMovement : NetworkComponent {
    public Transform Transform { get; set; }

    private bool amIOwner;
    private bool amISender;

    void Awake() {
        RegisterContext(this);

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
            Transforms.ToLocal(Transform, Context.Scene.transform),
            amIOwner
        );

        Context.SendJson(message);
    }

    public override void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        MovementMessage message = _message.FromJson<MovementMessage>();

        Pose pose = Transforms.ToWorld(message.Pose, Context.Scene.transform);
        Transform.SetPositionAndRotation(pose.position, pose.rotation);

        if (message.IsOwned) {
            // If object is taken by another, the current player is no longer the amISender
            amISender = false;
        }
    }
}
