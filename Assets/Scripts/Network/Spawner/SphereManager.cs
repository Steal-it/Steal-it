using System.Collections;
using Ubiq.Geometry;
using Ubiq.Messaging;
using Ubiq.Spawning;
using UnityEditor.XR.Interaction.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MovementMessage
{
    public Pose pose;
    public bool isFree;
}

public class SphereManager : MonoBehaviour, INetworkSpawnable
{
    private Rigidbody body;
    public NetworkId NetworkId {get;set;}

    public bool owner;
    public bool isFree;

    private NetworkContext context;
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        owner = false;
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("Object grabbed by: " + args.interactorObject.transform.name);
        if(isFree)
        {
            owner = true;
            isFree = false; 
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log("Object released");
        isFree = true;
    }

    private void Start()
    {
        context = NetworkScene.Register(this);
    }

    private void SendMessage()
    {
        var message = new MovementMessage();
        message.pose = Transforms.ToLocal(transform,context.Scene.transform);
        message.isFree = this.isFree;
        context.SendJson(message);
    }

    private void FixedUpdate()
    {
        if(owner)
        {
            SendMessage();
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<MovementMessage>();
        var pose = Transforms.ToWorld(msg.pose,context.Scene.transform);
        transform.position = pose.position;
        transform.rotation = pose.rotation;
        if(isFree && !msg.isFree)
        {
            Debug.Log("Change owner");
            //If I register the ball as free but I get a message saying is no longer free, this means another is now the owner, so set owner to false
            owner = false;
        }
        isFree = msg.isFree;
        if(!isFree && !owner)
        {
            Debug.Log("Disabled interaction: not Free and not Owned");
            body.useGravity = false;
            grabInteractable.enabled = false;
        }
        else if(isFree)
        {
            Debug.Log("Enabling interaction");
            body.useGravity = true;
            grabInteractable.enabled = true;
        }
    }
}
