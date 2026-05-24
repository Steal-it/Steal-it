using System.Collections;
using Ubiq.Geometry;
using Ubiq.Messaging;
using Ubiq.Spawning;
using UnityEditor.XR.Interaction.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

//If object is owned, transfer position, otherwise transfer velocity
public class MovementMessageV2
{
    public Vector3 velocity;
    public bool IsOwned;
}
public class SphereManagerV2 : MonoBehaviour, INetworkSpawnable
{
    public NetworkId NetworkId {get;set;}
    public bool AmIOwner;
    public bool isOwned;
    public string OriginalSender; //Only the original AmISender will have this populated

    private NetworkContext _context;
    private XRGrabInteractable _grabInteractable;
    private Rigidbody _body;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _grabInteractable = GetComponent<XRGrabInteractable>();
        AmIOwner = false;
        isOwned = false;
    }

    private void OnEnable()
    {
        _grabInteractable.selectEntered.AddListener(OnGrab);
        _grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void Start()
    {
        _context = NetworkScene.Register(this);
    }

    private void FixedUpdate()
    {
        //Only AmISender transmit Position, therefore, if I am not the AmISender I deactivate gravity
        if(AmIOwner)
        {
            _body.useGravity = true;
            SendMessage();
        }
        else
        {
            if(isOwned)
            {
                _body.useGravity = false;
            }
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("Object grabbed by: " + args.interactorObject.transform.name);
        AmIOwner = true;
        isOwned = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log("Object released");
        AmIOwner = false;
        isOwned = false;

        var msg = new MovementMessageV2();
        msg.velocity = GetComponent<Rigidbody>().linearVelocity;
        msg.IsOwned = false;

        _context.SendJson(msg);
    }

    private void SendMessage()
    {
        var message = new MovementMessageV2();
        message.velocity = GetComponent<Rigidbody>().linearVelocity;
        
        //When SendMessage is called, the instance is always the owner
        message.IsOwned = true;
        
        _context.SendJson(message);
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<MovementMessageV2>();

        var vel = msg.velocity;
        GetComponent<Rigidbody>().linearVelocity = vel;

        isOwned = msg.IsOwned;

        if (isOwned)
        {
            //If object is taken by another, The current player is no longer the AmISender
            _grabInteractable.enabled = false;
        }
        else
        {
            _grabInteractable.enabled = true;
        }
    }

    private void OnDisable()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrab);
        _grabInteractable.selectExited.RemoveListener(OnRelease);
    }
}