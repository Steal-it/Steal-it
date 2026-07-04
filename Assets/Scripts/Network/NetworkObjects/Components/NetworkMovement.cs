using SIPSorcery.Net;
using Ubiq.Geometry;
using Ubiq.Messaging;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class NetworkMovement : NetworkComponent {

    [SerializeField]
    public Transform Transform;

    private bool amIOwner;
    private bool amISender;

    private XRBaseInteractable interactable;

    void Awake() {
        RegisterContext(this);

        TryGetComponent(out interactable);
        amIOwner = false;

        if (Transform == null) {
            Transform = gameObject.transform;
        }
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

        if (Transform == null) {
            Debug.LogError("Error: cannot send transform info, transform is null");
            return;
        }

        MovementMessage message = new MovementMessage(
            Transforms.ToLocal(Transform, Context.Scene.transform),
            amIOwner
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
