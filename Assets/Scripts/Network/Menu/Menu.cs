using NUnit.Framework.Internal;
using Ubiq.Messaging;
using UnityEngine;
using Ubiq.Rooms;
using System;
using System.Threading.Tasks;

public class Menu : MonoBehaviour
{
    public Transform SpawnRelativeTransform;
    private NetworkScene _networkScene;
    private RoomClient _roomClient;

    [SerializeField]
    private GameObject _readyButton;
    [SerializeField]
    private MsgHandler _msgHandler;

    void Start() {
        _msgHandler = MsgHandler.Instance;
    }
    void OnEnable()
    {   
        _msgHandler.OnCounterRecoverFinished += OnCounterRecoverFinishedHandler;
    }

    void OnDestroy() {
        _msgHandler.OnCounterRecoverFinished -= OnCounterRecoverFinishedHandler;
    }
    public NetworkScene networkScene
    {
        get
        {
            if (!_networkScene)
            {
                _networkScene = NetworkScene.Find(this); //Find networkscene in parents
            }
            return _networkScene;
        }
    }
    public RoomClient roomClient
    {
        get
        {
            if (!_roomClient)
            {
                if (networkScene)
                {
                    _roomClient = networkScene.GetComponent<RoomClient>();
                }
            }
            return _roomClient;
        }
    }

    public void Request()
    {
        var cam = Camera.main.transform;
        transform.position = cam.TransformPoint(SpawnRelativeTransform.localPosition);
        transform.rotation = cam.rotation * SpawnRelativeTransform.localRotation;
        gameObject.SetActive(true);
    }

    private void OnCounterRecoverFinishedHandler(object sender, EventArgs e)
    {
        _readyButton.SetActive(true);
    }
}
