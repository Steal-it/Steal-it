using System;
using System.Linq;
using System.Threading.Tasks;
using TinyJson;
using Ubiq.Messaging;
using Ubiq.Rooms;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;



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
    public static MsgHandler Instance { get; private set; }

    public event EventHandler OnCounterRecoverFinished;
    [SerializeField]
    private Menu _mainMenu;

    [SerializeField]
    private LevelManager _levelManager;
    private NetworkContext _context;

    private bool _wasCounterRequested;
    private int _receiveReadyMsgCounter;
    private int _receiveLoadCompleteMsgCounter;

    private int _receiveRecoverCurrentCounterReplyCounter;

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
    }

    private void Start()
    {
        _context = NetworkScene.Register(this);
        _receiveReadyMsgCounter = 0;
        _receiveLoadCompleteMsgCounter = 0;
        _receiveRecoverCurrentCounterReplyCounter = 0;
        _mainMenu.roomClient.OnJoinedRoom.AddListener(OnJoinedRoomHandler);
    }

    private async void ChangeLevelHandler()
    {
        do {
            await Task.Delay(100);
        } while (_receiveReadyMsgCounter!=_mainMenu.roomClient.Peers.Count()+1);
        
        Debug.Log("All ready for change!");
        _receiveReadyMsgCounter = 0;
        _wasCounterRequested=false;
        _levelManager.LoadScreen("Test");
    }

    private async void PeerLoadingHandler()
    {
        do {
            await Task.Delay(100);
        } while (_receiveLoadCompleteMsgCounter!=_mainMenu.roomClient.Peers.Count()+1);
        
        Debug.Log("All ready for unlocking!");
        _receiveLoadCompleteMsgCounter = 0;
        _levelManager.UpdatePeerLoadingStatus();
    }

    private async void RecoverCurrentCounterRequestMessageHandler()
    {
        if(_context.Scene != null)
        {
            _context.SendJson<RecoverCurrentCounterReplyMessage>(new RecoverCurrentCounterReplyMessage(_receiveReadyMsgCounter));
        }
        else
        {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); //Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
            RecoverCurrentCounterRequestMessageHandler();
        }
        
    }

    private void RecoverCurrentCounterReplyMessageHandler(RecoverCurrentCounterReplyMessage msg)
    {
        if(_wasCounterRequested && msg.localCounter>_receiveReadyMsgCounter)
        {
            _receiveReadyMsgCounter = msg.localCounter;
        }
    }

    private async void RecoverCurrentCounterProcessStatusChecker()
    {
        //Check whether the recovery counter process has ended and signal registered handler

        do {
            await Task.Delay(100);
        }while(_receiveRecoverCurrentCounterReplyCounter == _mainMenu.roomClient.Peers.Count());

        _receiveRecoverCurrentCounterReplyCounter=0;
        OnCounterRecoverFinished?.Invoke(this, EventArgs.Empty);
    }

    public async void SendReadyMessage()
    {
        if(_context.Scene != null)
        {
            _receiveReadyMsgCounter+=1;
            _context.SendJson<ReadyMessage>(new ReadyMessage());
            if(_receiveReadyMsgCounter==1)
            {
                ChangeLevelHandler();
            }
        }
        else
        {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); //Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
            SendReadyMessage();
        }
    }

    public async void SendLoadLevelCompletedMessage()
    {
        if(_context.Scene != null)
        {
            _receiveLoadCompleteMsgCounter+=1;
            _context.SendJson<LoadLevelCompletedMessage>(new LoadLevelCompletedMessage());
            if(_receiveLoadCompleteMsgCounter==1)
            {
                PeerLoadingHandler();
            }
        } else {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); //Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
            SendLoadLevelCompletedMessage();
        }
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage msg)
    {
        BaseMessage message = msg.FromJson<BaseMessage>();
        switch(message.messageType)
        {
            case "ReadyMsg":
                Debug.Log("Received Ready Msg");
                _receiveReadyMsgCounter+=1;
                if(_receiveReadyMsgCounter==1)
                {
                    ChangeLevelHandler();
                }
                break;
            case "LoadLevelCompletedMsg":
                Debug.Log("Received Load Msg");
                _receiveLoadCompleteMsgCounter+=1;
                if(_receiveLoadCompleteMsgCounter==1)
                {
                    PeerLoadingHandler();
                }
                break;
            case "RecoverCurrentCounterRequestMsg":
                _wasCounterRequested = true;
                Debug.Log("Received counter request");
                RecoverCurrentCounterRequestMessageHandler();
                break;
            case "RecoverCurrentCounterReplyMsg":
                Debug.Log("Received counter reply");
                RecoverCurrentCounterReplyMessage finalMessage = msg.FromJson<RecoverCurrentCounterReplyMessage>();
                RecoverCurrentCounterReplyMessageHandler(finalMessage);
                _receiveRecoverCurrentCounterReplyCounter+=1;
                if(_receiveRecoverCurrentCounterReplyCounter==1)
                {
                    RecoverCurrentCounterProcessStatusChecker();
                }
                break;
            default:
                Debug.LogError("Received unknown message!"+message.messageType);
                break;
        }
    }

    private async void OnJoinedRoomHandler(IRoom arg)
    {
        if(_mainMenu.roomClient.Peers.Count()==0 && !arg.Name.IsNullOrEmpty())
        {
            OnCounterRecoverFinished?.Invoke(this,EventArgs.Empty);
            return;
        }
        if(_context.Scene != null)
        {
            _context.SendJson<RecoverCurrentCounterRequestMessage>(new RecoverCurrentCounterRequestMessage());
        }
        else
        {
            Debug.LogWarning("Network context is not available, retry send in one second");
            await Task.Delay(1000); //Wait a second before sending a message: this allow to be sure about a complete connection between a new peer and existing peers.
            OnJoinedRoomHandler(arg);
        }
    }

}