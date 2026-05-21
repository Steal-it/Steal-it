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
    public string messageType; //Need to be public due to serialization

    public BaseMessage(String msgType) {
        this.messageType = msgType;
    }
}

[System.Serializable]
public class ReadyMessage: BaseMessage
{
    public ReadyMessage() : base("ReadyMsg") {} 
}

[System.Serializable]
public class LoadLevelCompletedMessage: BaseMessage
{
    public LoadLevelCompletedMessage() : base("LoadLevelCompletedMsg") {}
}

[System.Serializable]
public class RecoverCurrentCounterRequestMessage: BaseMessage
{
    public RecoverCurrentCounterRequestMessage() : base("RecoverCurrentCounterRequestMsg") {}
}

[System.Serializable]
public class RecoverCurrentCounterReplyMessage: BaseMessage
{
    public int localCounter; //Need to be public due to serialization
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
    private int receiveReadyMsgCounter;
    public int receiveLoadCompleteMsgCounter {get; private set;}

    private void Start()
    {
        context = NetworkScene.Register(this);
        receiveReadyMsgCounter = 0;
        receiveLoadCompleteMsgCounter = 0;
    }

    private async void ChangeLevelHandler()
    {
        do {
            await Task.Delay(100);
        } while (receiveReadyMsgCounter!=mainMenu.roomClient.Peers.Count()+1);
        
        Debug.Log("All ready for change!");
        receiveReadyMsgCounter = 0;
        wasCounterRequested=false;
        levelManager.LoadScreen("Test");
    }

    private async void PeerLoadingHandler()
    {
        do {
            await Task.Delay(100);
        } while (receiveLoadCompleteMsgCounter!=mainMenu.roomClient.Peers.Count()+1);
        
        Debug.Log("All ready for unlocking!");
        receiveLoadCompleteMsgCounter = 0;
        levelManager.UpdatePeerLoadingStatus();
    }

    private void RecoverCurrentCounterRequestMessageHandler()
    {
        context.SendJson<RecoverCurrentCounterReplyMessage>(new RecoverCurrentCounterReplyMessage(receiveReadyMsgCounter));
    }

    private void RecoverCurrentCounterReplyMessageHandler(RecoverCurrentCounterReplyMessage msg)
    {
        if(wasCounterRequested && msg.localCounter>receiveReadyMsgCounter)
        {
            receiveReadyMsgCounter = msg.localCounter;
        }
    }

    public void SendReadyMessage()
    {
        if(context.Scene != null)
        {
            receiveReadyMsgCounter+=1;
            context.SendJson<ReadyMessage>(new ReadyMessage());
            if(receiveReadyMsgCounter==1)
            {
                ChangeLevelHandler();
            }
        }
    }

    public void SendLoadLevelCompletedMessage()
    {
        if(context.Scene != null)
        {
            receiveLoadCompleteMsgCounter+=1;
            context.SendJson<LoadLevelCompletedMessage>(new LoadLevelCompletedMessage());
            if(receiveLoadCompleteMsgCounter==1)
            {
                PeerLoadingHandler();
            }
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage msg)
    {
        BaseMessage message = msg.FromJson<BaseMessage>();
        switch(message.messageType)
        {
            case "ReadyMsg":
                Debug.Log("Received Ready Msg");
                receiveReadyMsgCounter+=1;
                if(receiveReadyMsgCounter==1)
                {
                    ChangeLevelHandler();
                }
                break;
            case "LoadLevelCompletedMsg":
                Debug.Log("Received Load Msg");
                receiveLoadCompleteMsgCounter+=1;
                if(receiveLoadCompleteMsgCounter==1)
                {
                    PeerLoadingHandler();
                }
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