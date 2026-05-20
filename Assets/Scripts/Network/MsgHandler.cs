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
    public string message;

    public BaseMessage(String msg) {
        this.message = msg;
    }
}

[System.Serializable]
public class ReadyMessage: BaseMessage {
    public ReadyMessage(String msg) : base(msg) {}
}

[System.Serializable]
public class LoadSceneCompletedMessage: BaseMessage {
    public LoadSceneCompletedMessage(String msg) : base(msg) {}
}

public class MsgHandler : MonoBehaviour {

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

    private async void ChangeLevelHandler()
    {
        do {
            await Task.Delay(100);
        } while (receiveCounter!=mainMenu.roomClient.Peers.Count()+1);
        
        Debug.Log("All ready!");
        receiveCounter = 0;
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

    public void SendReadyMessage()
    {
        if(context.Scene != null)
        {
            receiveCounter+=1;
            context.SendJson<ReadyMessage>(new ReadyMessage ("ReadyMsg"));
        }
    }

    public void SendLoadSceneCompletedMessage()
    {
        if(context.Scene != null)
        {
            receiveCounter+=1;
            context.SendJson<LoadSceneCompletedMessage>(new LoadSceneCompletedMessage ("LoadCompleteMsg"));
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage msg)
    {
        System.Object message = msg.FromJson<System.Object>();

        if(message is ReadyMessage)
        {
            Debug.Log("Received Ready Msg");
            receiveCounter+=1;
            if(receiveCounter==1)
            {
                ChangeLevelHandler();
            }
        }
        if(message is LoadSceneCompletedMessage)
        {
            Debug.Log("Received Load Msg");
            receiveCounter+=1;
            if(receiveCounter==1)
            {
                PeerLoadingHandler();
            }
        }
    }

}