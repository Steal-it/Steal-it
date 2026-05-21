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
}
