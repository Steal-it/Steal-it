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
    public Pose position;
    public bool isOwned;
}
public class SphereManager : MonoBehaviour, INetworkSpawnable
{
    private Rigidbody body;
    public NetworkId NetworkId {get;set;}

    public bool owner;

    public bool sender;

    public string originalSender;

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

    private void Update()
    {
        Menu mainMenu = FindFirstObjectByType<Menu>();
        if(mainMenu.roomClient.Me.uuid == originalSender)
        {
            Debug.Log("sender");
            sender = true;
        }
        Debug.Log(originalSender+"-2-"+mainMenu.roomClient.Me.uuid);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("Object grabbed by: " + args.interactorObject.transform.name);
        owner = true;
        sender = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log("Object released");
        owner = false;
        var msg = new MovementMessage();
        msg.position = Transforms.ToLocal(transform,context.Scene.transform);
        msg.isOwned = false;
        context.SendJson(msg);
    }

    private void Start()
    {
        context = NetworkScene.Register(this);
    }

    private void SendMessage()
    {
        var message = new MovementMessage();
        message.position = Transforms.ToLocal(transform,context.Scene.transform);
        message.isOwned = owner;
        context.SendJson(message);
    }

    private void FixedUpdate()
    {
        //Only sender transmit position, therefore, if I am not the sender I deactivate gravity
        if(sender)
        {
            body.isKinematic = false;
            SendMessage();
        }
        else
        {
            body.isKinematic = true;
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<MovementMessage>();
        var pose = Transforms.ToWorld(msg.position,context.Scene.transform);
        transform.position = pose.position;
        transform.rotation = pose.rotation;

        if(msg.isOwned)
        {
            //If object is taken by another, The current player is no longer the sender
            sender=false;
            grabInteractable.enabled = false;
        }
        else
        {
            grabInteractable.enabled = true;
        }
    }
}


/*public class MovementMessage
{
    public Pose pose;
    public bool free;
}
public class SphereManager : MonoBehaviour, INetworkSpawnable
{
    private Rigidbody body;
    public NetworkId NetworkId {get;set;}

    public bool owner;

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
        owner = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log("Object released");
        owner = false;
        var msg = new MovementMessage();
        msg.pose = Transforms.ToLocal(transform,context.Scene.transform);
        msg.free = true;
        context.SendJson(msg);
    }

    private void Start()
    {
        context = NetworkScene.Register(this);
    }

    private void SendMessage()
    {
        var message = new MovementMessage();
        message.pose = Transforms.ToLocal(transform,context.Scene.transform);
        message.free = false;
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
        if(!owner)
        {
            Debug.Log("Disabled interaction: not Free and not Owned");
            body.isKinematic = true;
            grabInteractable.enabled = false;
        }
        else
        {
            Debug.Log("Enabling interaction");
            body.isKinematic = false;
            grabInteractable.enabled = true;
        }

        if(msg.free)
        {
            Debug.Log("Enabling interaction");
            body.isKinematic = false;
            grabInteractable.enabled = true;
        }
    }
}*/
