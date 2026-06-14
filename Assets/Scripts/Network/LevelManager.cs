using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

public class LevelManager : MonoBehaviour {
    public event EventHandler<OnGameLoadedEventArgs> OnGameLoaded;
    public class OnGameLoadedEventArgs : EventArgs {
        public bool IsClientAsServer;
    }

    [SerializeField]
    private XROrigin rig;
    [SerializeField]
    private LocomotionMediator rigLocomotor;
    [SerializeField]
    private Transform localLobbySpawnPoint;
    [SerializeField]
    private Transform roomLobbySpawnPoint;
    [SerializeField]
    private Transform gameSpawnPoint;
    [SerializeField]
    private GameObject loaderCanvas;
    [SerializeField]
    private Image progressBar;

    private MsgHandler msgHandler;
    private LocalLobbyMenu localLobbyMenu;
    private RoomLobbyMenu roomLobbyMenu;
    private bool isClientAsServer;

    private float target;
    // private Boolean hadAllPeerLoadedScene;

    void Start() {
        msgHandler = NetworkReferenceManager.Instance.MsgHandler;
        localLobbyMenu = NetworkReferenceManager.Instance.LocalLobbyMenu;
        roomLobbyMenu = NetworkReferenceManager.Instance.RoomLobbyMenu;

        msgHandler.OnAllPeersReadyForChange += LoadScreen;
        msgHandler.OnAllPeersLoadingLevelFinished += UpdatePeerLoadingStatus;
        localLobbyMenu.OnNewRoomCreated += MainMenu_OnNewRoomCreated;
        roomLobbyMenu.OnRoomExited += RoomLobbyMenu_OnRoomExited;

        LoadLocalLobby();
    }

    void Update() {
        progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, target, 3 * Time.deltaTime);
    }

    private void LoadScreen(object _sender, MsgHandler.OnAllPeersReadyForChangeEventArgs _event) {
        target = 0;
        progressBar.fillAmount = 0;

        //loaderCanvas.SetActive(true);
        // hadAllPeerLoadedScene = false;

        switch (_event.levelName) {
            case "Test":
                LoadGame();
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

    private void UpdatePeerLoadingStatus(object _sender, EventArgs _event) {
        // hadAllPeerLoadedScene = true;
    }

    private void MainMenu_OnNewRoomCreated(object _sender, EventArgs _event) {
        LoadRoomLobby();
        isClientAsServer = true;
    }

    private void RoomLobbyMenu_OnRoomExited(object _sender, EventArgs _event) {
        LoadLocalLobby();
        isClientAsServer = false;
    }

    private void LoadLocalLobby() {
        rig.transform.SetPositionAndRotation(localLobbySpawnPoint.position, localLobbySpawnPoint.rotation);
        rigLocomotor.gameObject.SetActive(false);
    }

    private void LoadRoomLobby() {
        rig.transform.SetPositionAndRotation(roomLobbySpawnPoint.position, roomLobbySpawnPoint.rotation);
        rigLocomotor.gameObject.SetActive(true);
    }

    private void LoadGame() {
        rig.transform.SetPositionAndRotation(gameSpawnPoint.position, gameSpawnPoint.rotation);
        rigLocomotor.gameObject.SetActive(true);

        OnGameLoaded?.Invoke(this, new OnGameLoadedEventArgs {
            IsClientAsServer = isClientAsServer
        });
    }

    void OnDestroy() {
        msgHandler.OnAllPeersReadyForChange -= LoadScreen;
        msgHandler.OnAllPeersLoadingLevelFinished -= UpdatePeerLoadingStatus;
        localLobbyMenu.OnNewRoomCreated -= MainMenu_OnNewRoomCreated;
        roomLobbyMenu.OnRoomExited -= RoomLobbyMenu_OnRoomExited;
    }
}
