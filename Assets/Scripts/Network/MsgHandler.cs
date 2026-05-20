using System;
using System.Linq;
using System.Threading.Tasks;
using TinyJson;
using Ubiq.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;



[System.Serializable]
public class BaseMessage
{
    public string messageType {get; set;}

    public BaseMessage(String msgType) {
        this.messageType = msgType;
    }
}

[System.Serializable]
public class ReadyMessage: BaseMessage
{
    public ReadyMessage() : base("ReadyMsg") {} //Need to be public due to serialization
}

[System.Serializable]
public class LoadSceneCompletedMessage: BaseMessage
{
    public LoadSceneCompletedMessage() : base("LoadSceneCompletedMsg") {}
}

[System.Serializable]
public class RecoverCurrentCounterRequestMessage: BaseMessage
{
    public RecoverCurrentCounterRequestMessage() : base("RecoverCurrentCounterRequestMsg") {}
}

[System.Serializable]
public class RecoverCurrentCounterReplyMessage: BaseMessage
{
    public int localCounter {get; set;} //Need to be public due to serialization
    public RecoverCurrentCounterReplyMessage(int localCounter) : base("RecoverCurrentCounterReplyMsg")
    {
        this.localCounter = localCounter;
    }
}

public class MsgHandler : MonoBehaviour
{

    public Menu mainMenu;

    [SerializeField]
    private LevelManager levelManager;
    private NetworkContext context;

    private bool wasCounterRequested;
    private int receiveCounter;

    private void Start()
    {
        context = NetworkScene.Register(this);
        receiveCounter = 0;
    }

    private async void ChangeLevelHandler()
    {
        do {
            await Task.Delay(100);
        } while (receiveCounter!=mainMenu.roomClient.Peers.Count()+1);
        
        Debug.Log("All ready!");
        receiveCounter = 0;
        wasCounterRequested=false;
        levelManager.LoadScreen("Test");
    }

    private async void PeerLoadingHandler()
    {
        do {
            await Task.Delay(100);
        } while (receiveCounter!=mainMenu.roomClient.Peers.Count()+1);
        
        Debug.Log("All ready!");
        receiveCounter = 0;
        levelManager.UpdatePeerLoadingStatus();
    }

    private void RecoverCurrentCounterRequestMessageHandler()
    {
        context.SendJson<RecoverCurrentCounterReplyMessage>(new RecoverCurrentCounterReplyMessage(this.receiveCounter));
    }

    private void RecoverCurrentCounterReplyMessageHandler(RecoverCurrentCounterReplyMessage msg)
    {
        if(wasCounterRequested && msg.localCounter>this.receiveCounter)
        {
            this.receiveCounter = msg.localCounter;
        }
    }

    public void SendReadyMessage()
    {
        if(context.Scene != null)
        {
            receiveCounter+=1;
            context.SendJson<ReadyMessage>(new ReadyMessage());
            if(receiveCounter==1)
            {
                ChangeLevelHandler();
            }
        }
    }

    public void SendLoadSceneCompletedMessage()
    {
        if(context.Scene != null)
        {
            receiveCounter+=1;
            context.SendJson<LoadSceneCompletedMessage>(new LoadSceneCompletedMessage());
            if(receiveCounter==1)
            {
                PeerLoadingHandler();
            }
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage msg)
    {
        BaseMessage message = msg.FromJson<BaseMessage>();
        Debug.Log("message: "+message.messageType+"!");
        switch(message.messageType)
        {
            case "ReadyMsg":
                Debug.Log("Received Ready Msg");
                ChangeLevelHandler();
                break;
            case "LoadSceneCompletedMsg":
                Debug.Log("Received Load Msg");
                PeerLoadingHandler();
                break;
            case "RecoverCurrentCounterRequestMsg":
                wasCounterRequested = true;
                Debug.Log("Received counter request");
                RecoverCurrentCounterRequestMessageHandler();
                break;
            case "RecoverCurrentCounterReplyMsg":
                Debug.Log("Received counter reply");
                RecoverCurrentCounterReplyMessage finalMessage = msg.FromJson<RecoverCurrentCounterReplyMessage>();
                RecoverCurrentCounterReplyMessageHandler(finalMessage);
                break;
            default:
                Debug.LogError("Received unknown message!"+message.messageType);
                break;
        }
    }

}