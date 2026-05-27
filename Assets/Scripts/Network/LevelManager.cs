using System;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public static LevelManager LvlManager;

    [SerializeField]
    private Transform rigTransformer;

    [SerializeField]
    private GameObject loaderCanvas;

    [SerializeField]
    private UnityEngine.UI.Image progressBar;

    [SerializeField]
    private MsgHandler msgHandler;

    private float target;
    // private Boolean hadAllPeerLoadedScene;

    void Awake() {
        if (LvlManager == null) {
            LvlManager = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        msgHandler.OnAllPeersReadyForChange += LoadScreen;
        msgHandler.OnAllPeersLoadingLevelFinished += UpdatePeerLoadingStatus;
    }

    void Update() {
        progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, target, 3 * Time.deltaTime);
    }

    public void LoadScreen(object sender, MsgHandler.OnAllPeersReadyForChangeEventArgs e) {
        target = 0;
        progressBar.fillAmount = 0;

        //loaderCanvas.SetActive(true);
        // hadAllPeerLoadedScene = false;

        switch (e.levelName) {
            case "Test":
                rigTransformer.position = new Vector3(17f, 2f, 2f);
                break;
            default:
                Debug.LogError("Attempt to switch to " + e.levelName + " but it does not exists");
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

    public void UpdatePeerLoadingStatus(object sender, EventArgs e) {
        // hadAllPeerLoadedScene = true;
    }

    void OnDestroy() {
        msgHandler.OnAllPeersReadyForChange -= LoadScreen;
        msgHandler.OnAllPeersLoadingLevelFinished -= UpdatePeerLoadingStatus;
    }
}
