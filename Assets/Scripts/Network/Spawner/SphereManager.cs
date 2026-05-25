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
    public Pose Position;
    public bool IsOwned;
}
public class SphereManager : MonoBehaviour, INetworkSpawnable
{
    public NetworkId NetworkId {get;set;}
    public string OriginalSender {private get; set;} //Only the original amISender will have this populated
    private bool amIOwner;
    private bool amISender;
    private bool hasOriginalSenderCheckBeenPerformed;

    private NetworkContext context;
    private XRGrabInteractable grabInteractable;
    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        amIOwner = false;
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
        //Unfortunately, putting this in Start method will not work
        if(!hasOriginalSenderCheckBeenPerformed)
        {
            Menu mainMenu = FindFirstObjectByType<Menu>(); //Since this object is a prefab, the following search is necessary
            if(mainMenu.RoomClient.Me.uuid == OriginalSender)
            {
                Debug.Log("amISender");
                amISender = true;
            }
            Debug.Log(OriginalSender+"-2-"+mainMenu.RoomClient.Me.uuid);
            hasOriginalSenderCheckBeenPerformed=true;
        }
    }

    private void OnGrab(SelectEnterEventArgs _args)
    {
        Debug.Log("Object grabbed by: " + _args.interactorObject.transform.name);
        amIOwner = true;
        amISender = true;
    }

    private void OnRelease(SelectExitEventArgs _args)
    {
        Debug.Log("Object released");
        amIOwner = false;
        var msg = new MovementMessage();
        msg.Position = Transforms.ToLocal(transform,context.Scene.transform);
        msg.IsOwned = false;
        context.SendJson(msg);
    }

    private void Start()
    {
        context = NetworkScene.Register(this);
    }

    private void SendMessage()
    {
        var message = new MovementMessage();
        message.Position = Transforms.ToLocal(transform,context.Scene.transform);
        message.IsOwned = amIOwner;
        Debug.Log("Sending" + message.IsOwned);
        context.SendJson(message);
    }

    private void FixedUpdate()
    {
        //Only amISender transmit Position, therefore, if I am not the amISender I deactivate gravity
        if(amISender)
        {
            body.useGravity = true;
            SendMessage();
        }
        else
        {
            body.useGravity = false;
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage _message)
    {
        Debug.Log("receiving");
        var msg = _message.FromJson<MovementMessage>();
        var pose = Transforms.ToWorld(msg.Position,context.Scene.transform);
        transform.position = pose.position;
        transform.rotation = pose.rotation;

        if(msg.IsOwned)
        {
            //If object is taken by another, The current player is no longer the amISender
            amISender=false;
            grabInteractable.enabled = false;
        }
        else
        {
            grabInteractable.enabled = true;
        }
    }
}