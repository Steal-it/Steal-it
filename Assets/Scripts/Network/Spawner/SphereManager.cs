using System.Collections;
using Ubiq.Geometry;
using Ubiq.Messaging;
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
    public bool AmIOwner;

    public bool AmISender;

    public string OriginalSender; //Only the original AmISender will have this populated

    private bool _hasOriginalSenderCheckBeenPerformed;

    private NetworkContext _context;
    private XRGrabInteractable _grabInteractable;
    private Rigidbody _body;

    private void Awake() {
        _body = GetComponent<Rigidbody>();
        _grabInteractable = GetComponent<XRGrabInteractable>();
        AmIOwner = false;
        _hasOriginalSenderCheckBeenPerformed = false;
    }

    private void OnEnable() {
        _grabInteractable.selectEntered.AddListener(OnGrab);
        _grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable() {
        _grabInteractable.selectEntered.RemoveListener(OnGrab);
        _grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void Update() {
        if (!_hasOriginalSenderCheckBeenPerformed) {
            Menu mainMenu = FindFirstObjectByType<Menu>();
            if (mainMenu.roomClient.Me.uuid == OriginalSender) {
                Debug.Log("AmISender");
                AmISender = true;
            }
            Debug.Log(OriginalSender + "-2-" + mainMenu.roomClient.Me.uuid);
            _hasOriginalSenderCheckBeenPerformed = true;
        }
    }

    private void OnGrab(SelectEnterEventArgs args) {
        Debug.Log("Object grabbed by: " + args.interactorObject.transform.name);
        AmIOwner = true;
        AmISender = true;
    }

    private void OnRelease(SelectExitEventArgs args) {
        Debug.Log("Object released");
        AmIOwner = false;
        var msg = new MovementMessage();
        msg.Position = Transforms.ToLocal(transform, _context.Scene.transform);
        msg.IsOwned = false;
        _context.SendJson(msg);
    }

    private void Start() {
        _context = NetworkScene.Register(this);
    }

    private void SendMessage() {
        var message = new MovementMessage();
        message.Position = Transforms.ToLocal(transform, _context.Scene.transform);
        message.IsOwned = AmIOwner;
        _context.SendJson(message);
    }

    private void FixedUpdate() {
        //Only AmISender transmit Position, therefore, if I am not the AmISender I deactivate gravity
        if (AmISender) {
            _body.useGravity = true;
            SendMessage();
        } else {
            _body.useGravity = false;
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message) {
        var msg = message.FromJson<MovementMessage>();
        var pose = Transforms.ToWorld(msg.Position, _context.Scene.transform);
        transform.position = pose.position;
        transform.rotation = pose.rotation;

        if (msg.IsOwned) {
            //If object is taken by another, The current player is no longer the AmISender
            AmISender = false;
            _grabInteractable.enabled = false;
        } else {
            _grabInteractable.enabled = true;
        }
    }
}