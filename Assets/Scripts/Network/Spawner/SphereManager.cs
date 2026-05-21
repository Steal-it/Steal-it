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

    public string originalSender; //Only the original sender will have this populated

    public bool hasOriginalSenderCheckBeenPerformed;

    private NetworkContext context;
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        owner = false;
        hasOriginalSenderCheckBeenPerformed=false;
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
        if(!hasOriginalSenderCheckBeenPerformed)
        {
            Menu mainMenu = FindFirstObjectByType<Menu>();
            if(mainMenu.roomClient.Me.uuid == originalSender)
            {
                Debug.Log("sender");
                sender = true;
            }
            Debug.Log(originalSender+"-2-"+mainMenu.roomClient.Me.uuid);
            hasOriginalSenderCheckBeenPerformed=true;
        }
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