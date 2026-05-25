using NUnit.Framework.Internal;
using Ubiq.Messaging;
using UnityEngine;
using Ubiq.Rooms;
using System;
using System.Threading.Tasks;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private RoomClient roomClient;

    //Provides access to roomClient to all children
    public RoomClient RoomClient
    {
        get
        {
            return roomClient;
        }
        private set
        {
            roomClient = value;
        }
    }

    [SerializeField]
    private GameObject readyButton;
    [SerializeField]
    private MsgHandler msgHandler;
    [SerializeField]
    private Transform spawnRelativeTransform;

    void Start()
    {
        msgHandler = MsgHandler.Instance;
    }
    void OnEnable()
    {   
        msgHandler.OnCounterRecoverFinished += OnCounterRecoverFinishedHandler;
    }

    void OnDestroy()
    {
        msgHandler.OnCounterRecoverFinished -= OnCounterRecoverFinishedHandler;
    }

    public void Request()
    {
        var cam = Camera.main.transform;
        transform.position = cam.TransformPoint(spawnRelativeTransform.localPosition);
        transform.rotation = cam.rotation * spawnRelativeTransform.localRotation;
        gameObject.SetActive(true);
    }

    private void OnCounterRecoverFinishedHandler(object _sender, EventArgs _e)
    {
        readyButton.SetActive(true);
    }
}
