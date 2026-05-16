using NUnit.Framework.Internal;
using Ubiq.Messaging;
using UnityEngine;
using Ubiq.Rooms;

public class Menu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private NetworkScene _networkScene;
    public Transform spawnRelativeTransform;
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

    private RoomClient _roomClient;
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

    public void Request ()
    {
        var cam = Camera.main.transform;
        transform.position = cam.TransformPoint(spawnRelativeTransform.localPosition);
        transform.rotation = cam.rotation * spawnRelativeTransform.localRotation;
        gameObject.SetActive(true);
    }
    
}
