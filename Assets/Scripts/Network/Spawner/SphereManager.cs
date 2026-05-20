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
}

public class SphereManager : MonoBehaviour, INetworkSpawnable
{
    private Rigidbody body;
    public NetworkId NetworkId {get;set;}

    public bool owner;

    private NetworkContext context;

    private Vector3 position;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        owner = false;
    }

    private void Start()
    {
        context = NetworkScene.Register(this);

        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        grab.selectExited.AddListener(EndGrabHandler);
        grab.selectEntered.AddListener(StartGrabHandler);
    }

    private void EndGrabHandler(SelectExitEventArgs eventArgs)
    {
        var interactable = (XRGrabInteractable)eventArgs.interactableObject;
        interactable.enabled = true;
        owner = false;
    }

    private void StartGrabHandler(SelectEnterEventArgs eventArgs)
    {
        var interactable = (XRGrabInteractable)eventArgs.interactableObject;
        interactable.enabled = false;
        owner = true;
    }

    private void SendMessage()
    {
        Debug.Log("Sending");
        var message = new MovementMessage();
        message.pose = Transforms.ToLocal(transform,context.Scene.transform);
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
    }
}
