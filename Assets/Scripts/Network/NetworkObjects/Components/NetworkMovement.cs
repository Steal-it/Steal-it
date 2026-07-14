using Ubiq.Geometry;
using Ubiq.Messaging;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class NetworkMovement : NetworkComponent {
    [SerializeField]
    public Transform Transform;

    public bool AmIOwner { get; private set; }
    public bool AmISender { get; private set; }

    private XRBaseInteractable interactable;

    void Start() {
        RegisterContext(this);

        TryGetComponent(out interactable);
        AmIOwner = false;

        if (Transform == null) {
            Transform = transform;
        }
    }

    void FixedUpdate() {
        // Only if I am the sender I transmit the position
        if (AmISender) {
            SendMovementMessage();
        }
    }

    public void SelectObject() {
        AmIOwner = true;
        AmISender = true;
        SendMovementMessage();
    }

    public void ReleaseObject() {
        AmIOwner = false;
        SendMovementMessage();
    }

    public void DeselectObject() {
        AmIOwner = false;
        AmISender = false;
        SendMovementMessage();
    }

    private void SendMovementMessage() {
        if (Transform == null) {
            Debug.LogWarning("Error: cannot send transform info, transform is null");
            return;
        }

        MovementMessage message = new MovementMessage(
            Transforms.ToLocal(Transform, Context.Scene.transform),
            AmIOwner
        );

        Context.SendJson(message);
    }

    public override void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        MovementMessage message = _message.FromJson<MovementMessage>();

        if (interactable != null) {
            // If an interactable is present and owner is set a true the interactable has to be disabled and viceversa
            interactable.enabled = !message.IsOwned;
        }

        Pose pose = Transforms.ToWorld(message.Pose, Context.Scene.transform);
        Transform.SetPositionAndRotation(pose.position, pose.rotation);

        if (TryGetComponent(out Rigidbody rb)) { // disable gravity if rigidbody is present
            rb.useGravity = !message.IsOwned;
        }

        if (message.IsOwned) {
            // If object is taken by another, the current player is no longer the amISender
            AmISender = false;
        }
    }

    void OnDestroy() {
        if (interactable) {
            interactable.selectEntered.RemoveAllListeners();
            interactable.selectExited.RemoveAllListeners();
        }
    }
}
