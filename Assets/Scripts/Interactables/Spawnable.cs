using Ubiq.Geometry;
using Ubiq.Messaging;
using Ubiq.Spawning;
using UnityEngine;

public class Spawnable : MonoBehaviour, INetworkSpawnable {

    public NetworkId NetworkId { get; set; }

    private NetworkContext context;

    private bool owner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private struct Message {
        public Pose pose;
    }

    private void SendMessage() {
        var message = new Message();
        message.pose = Transforms.ToLocal(transform, context.Scene.transform);
        context.SendJson(message);
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message) {
        var msg = message.FromJson<Message>();
        var pose = Transforms.ToWorld(msg.pose, context.Scene.transform);
        transform.position = pose.position;
        transform.rotation = pose.rotation;
    }
}
