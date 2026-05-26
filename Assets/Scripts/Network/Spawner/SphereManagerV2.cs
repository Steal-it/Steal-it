using Ubiq.Geometry;
using Ubiq.Messaging;
using Ubiq.Spawning;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

//If object is owned, transfer position, otherwise transfer velocity
public class MovementMessageV2 {
    public Vector3 velocity;
    public Pose Position;
    public bool IsOwned;
}
public class SphereManagerV2 : MonoBehaviour, INetworkSpawnable {
    public NetworkId NetworkId { get; set; }
    public bool AmIOwner;
    public bool isOwned;
    public string OriginalSender; //Only the original AmISender will have this populated

    private NetworkContext _context;
    private XRGrabInteractable _grabInteractable;
    private Rigidbody _body;
    private Vector3 _lastPos;
    private Vector3 _vel;

    private void Awake() {
        _body = GetComponent<Rigidbody>();
        _grabInteractable = GetComponent<XRGrabInteractable>();
        AmIOwner = false;
        isOwned = false;
    }

    private void OnEnable() {
        _grabInteractable.selectEntered.AddListener(OnGrab);
        _grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void Start() {
        _context = NetworkScene.Register(this);
    }

    private void FixedUpdate() {
        _vel = (Transforms.ToLocal(transform, _context.Scene.transform).position - _lastPos) / Time.fixedDeltaTime;

        //Only AmISender transmit Position, therefore, if I am not the AmISender I deactivate gravity
        if (AmIOwner) {
            SendMessage();
        }

        _lastPos = Transforms.ToLocal(transform, _context.Scene.transform).position;
    }

    private void OnGrab(SelectEnterEventArgs args) {
        Debug.Log("Object grabbed by: " + args.interactorObject.transform.name);
        AmIOwner = true;
        isOwned = true;
    }

    private void OnRelease(SelectExitEventArgs args) {
        Debug.Log("Object released");
        AmIOwner = false;
        isOwned = false;

        var msg = new MovementMessageV2();
        //msg.velocity = GetComponent<Rigidbody>().linearVelocity;
        msg.velocity = _vel;
        print("REL: " + _vel);
        //msg.Position = new Pose(transform.position, transform.rotation);
        msg.Position = Transforms.ToLocal(transform, _context.Scene.transform);
        msg.IsOwned = false;

        _context.SendJson(msg);
    }

    private void SendMessage() {
        var message = new MovementMessageV2();

        //message.Position = new Pose(transform.position, transform.rotation);
        message.Position = Transforms.ToLocal(transform, _context.Scene.transform);

        //When SendMessage is called, the instance is always the owner
        message.IsOwned = true;

        _context.SendJson(message);
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message) {
        var msg = message.FromJson<MovementMessageV2>();

        isOwned = msg.IsOwned;

        if (isOwned) {
            //If object is taken by another, disable interaction and gravity
            _grabInteractable.enabled = false;
            _body.useGravity = false;
        } else {
            _grabInteractable.enabled = true;
            _body.useGravity = true;
        }

        // if(msg.velocity != Vector3.zero)
        // {
        var vel = msg.velocity;
        GetComponent<Rigidbody>().linearVelocity = vel;
        print("NEW: " + GetComponent<Rigidbody>().linearVelocity);
        // }

        var pose = Transforms.ToWorld(msg.Position, _context.Scene.transform);
        //var pose = msg.Position;
        transform.position = pose.position;
        transform.rotation = pose.rotation;

        /*if(msg.velocity == Vector3.zero)
        {
            var pose = Transforms.ToWorld(msg.Position,_context.Scene.transform);
            transform.position = pose.position;
            transform.rotation = pose.rotation;
        }
        else
        {
            var vel = msg.velocity;
            GetComponent<Rigidbody>().linearVelocity = vel;
        }*/

    }

    private void OnDisable() {
        _grabInteractable.selectEntered.RemoveListener(OnGrab);
        _grabInteractable.selectExited.RemoveListener(OnRelease);
    }
}