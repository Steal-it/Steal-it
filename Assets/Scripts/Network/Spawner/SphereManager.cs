using Ubiq.Geometry;
using Ubiq.Messaging;
using Ubiq.Rooms;
using Ubiq.Spawning;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MovementMessage {
    public Pose Position;
    public bool IsOwned;
}

public class SphereManager : MonoBehaviour, INetworkSpawnable {
    public NetworkId NetworkId { get; set; }
    public string OriginalSender { private get; set; } //Only the original amISender will have this populated
    private bool amIOwner;
    private bool amISender;
    private bool hasOriginalSenderCheckBeenPerformed;

    private RoomClient roomClient;
    private NetworkContext context;
    private XRGrabInteractable grabInteractable;
    private Rigidbody body;

    private void Awake() {
        body = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        amIOwner = false;
        hasOriginalSenderCheckBeenPerformed = false;
    }

    private void OnEnable() {
        roomClient = NetworkReferenceManager.Instance.RoomClient;

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void Start() {
        context = NetworkScene.Register(this);
    }

    private void Update() {
        //Unfortunately, putting this in Start method will not work
        if (!hasOriginalSenderCheckBeenPerformed) {
            if (roomClient.Me.uuid == OriginalSender) {
                Debug.Log("amISender");
                amISender = true;
            }
            Debug.Log(OriginalSender + "-2-" + roomClient.Me.uuid);
            hasOriginalSenderCheckBeenPerformed = true;
        }
    }

    private void OnGrab(SelectEnterEventArgs _args) {
        Debug.Log("Object grabbed by: " + _args.interactorObject.transform.name);
        amIOwner = true;
        amISender = true;
    }

    private void OnRelease(SelectExitEventArgs _args) {
        Debug.Log("Object released");
        amIOwner = false;
        var msg = new MovementMessage {
            Position = Transforms.ToLocal(transform, context.Scene.transform),
            IsOwned = false
        };
        context.SendJson(msg);
    }

    private void SendMessage() {
        var message = new MovementMessage {
            Position = Transforms.ToLocal(transform, context.Scene.transform),
            IsOwned = amIOwner
        };
        Debug.Log("Sending" + message.IsOwned);
        context.SendJson(message);
    }

    private void FixedUpdate() {
        //Only amISender transmit Position, therefore, if I am not the amISender I deactivate gravity
        if (amISender) {
            body.useGravity = true;
            SendMessage();
        } else {
            body.useGravity = false;
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        Debug.Log("receiving");
        var msg = _message.FromJson<MovementMessage>();
        var pose = Transforms.ToWorld(msg.Position, context.Scene.transform);
        transform.SetPositionAndRotation(pose.position, pose.rotation);

        if (msg.IsOwned) {
            //If object is taken by another, The current player is no longer the amISender
            amISender = false;
            grabInteractable.enabled = false;
        } else {
            grabInteractable.enabled = true;
        }
    }

    private void OnDisable() {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }
}