using System;
using System.Linq;
using TinyJson;
using Ubiq.Messaging;
using UnityEngine;

[System.Serializable]
public class ReadyMessage
{
    public string message;
}
public class ReadyMsgHandler : MonoBehaviour {

    public Menu mainMenu;
    private NetworkContext context;

    private int receiveCounter;

    private void Start()
    {
        context = NetworkScene.Register(this);
        receiveCounter = 0;
    }

    public void SendReadyMsg()
    {
        if(context.Scene != null)
        {
            Debug.Log("Send");
            context.SendJson<ReadyMessage>(new ReadyMessage { message = "ReadyMsg" });
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage msg)
    {
        ReadyMessage message = msg.FromJson<ReadyMessage>();
        if(message.message.Contains("ReadyMsg"))
        {
            Debug.Log("Received Msg");
            receiveCounter+=1;
        }

        if(receiveCounter==mainMenu.roomClient.Peers.Count())
        {
            Debug.Log("All ready!");
        }
    }
}