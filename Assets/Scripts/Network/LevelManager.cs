using System;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    [SerializeField]
    private Transform rigTransformer;
    [SerializeField]
    private Transform gameSpawnPoint;
    [SerializeField]
    private GameObject loaderCanvas;
    [SerializeField]
    private UnityEngine.UI.Image progressBar;

    private MsgHandler msgHandler;

    private float target;
    // private Boolean hadAllPeerLoadedScene;

    void Start() {
        msgHandler = NetworkReferenceManager.Instance.MsgHandler;

        msgHandler.OnAllPeersReadyForChange += LoadScreen;
        msgHandler.OnAllPeersLoadingLevelFinished += UpdatePeerLoadingStatus;
    }

    void Update() {
        progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, target, 3 * Time.deltaTime);
    }

    public void LoadScreen(object _sender, MsgHandler.OnAllPeersReadyForChangeEventArgs _event) {
        target = 0;
        progressBar.fillAmount = 0;

        //loaderCanvas.SetActive(true);
        // hadAllPeerLoadedScene = false;

        switch (_event.levelName) {
            case "Test":
                rigTransformer.position = gameSpawnPoint.position;
                break;
            default:
                Debug.LogError("Attempt to switch to " + _event.levelName + " but it does not exists");
                return;
        }


        msgHandler.SendLoadLevelCompletedMessage();

        /*int oldCounter = 0;
        int totalNumberOfPeers = 0;
        float increaseOf = 0;

        do {
            oldCounter = msgHandler.receiveLoadCompleteMsgCounter;
            await Task.Delay(100);
            totalNumberOfPeers = msgHandler.mainMenu.roomClient.Peers.Count()+1;
            increaseOf = 1/totalNumberOfPeers;
            if(oldCounter < msgHandler.receiveLoadCompleteMsgCounter)
            {
                target += increaseOf;
            }
        } while(!hadAllPeerLoadedScene);*/

        // hadAllPeerLoadedScene = false;

        loaderCanvas.SetActive(false);

    }

    public void UpdatePeerLoadingStatus(object _sender, EventArgs _event) {
        // hadAllPeerLoadedScene = true;
    }

    void OnDestroy() {
        msgHandler.OnAllPeersReadyForChange -= LoadScreen;
        msgHandler.OnAllPeersLoadingLevelFinished -= UpdatePeerLoadingStatus;
    }
}
