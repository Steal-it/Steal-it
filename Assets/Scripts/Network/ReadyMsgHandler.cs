using System;
using System.Linq;
using TinyJson;
using Ubiq.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;



[System.Serializable]
public class ReadyMessage
{
    public string message;
}
public class ReadyMsgHandler : MonoBehaviour {

    public Menu mainMenu;

    [SerializeField]
    private LevelManager levelManager;
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
            receiveCounter+=1;
            context.SendJson<ReadyMessage>(new ReadyMessage { message = "ReadyMsg" });
        }
        if(receiveCounter==mainMenu.roomClient.Peers.Count()+1)
        {
            Debug.Log("All ready!");
            levelManager.LoadScreen("Test");
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

        if(receiveCounter==mainMenu.roomClient.Peers.Count()+1)
        {
            Debug.Log("All ready!");
        }
    }
}