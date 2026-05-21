using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    public static LevelManager lvlManager;

    public Transform rigTransformer;

    public TextMeshProUGUI loadingInfoText;

    [SerializeField]
    private GameObject _loaderCanvas;

    [SerializeField]
    private UnityEngine.UI.Image _progressBar;

    private float _target;
    
    private Boolean hadAllPeerLoadedScene;

    [SerializeField]
    private MsgHandler msgHandler;
    void Awake() {
        if(lvlManager == null)
        {
            lvlManager = this;
            DontDestroyOnLoad(gameObject);
        } 
        else
        {
            Destroy(gameObject);
        }
    }
    
    public async void LoadScreen(string levelName)
    {
        _target = 0;
        _progressBar.fillAmount = 0;

        _loaderCanvas.SetActive(true);
        hadAllPeerLoadedScene = false;

        rigTransformer.position += Vector3.right * 19;

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
                _target += increaseOf;
            }
        } while(!hadAllPeerLoadedScene);*/

        hadAllPeerLoadedScene = false;

        _loaderCanvas.SetActive(false);
    
    }

    public void UpdatePeerLoadingStatus()
    {
        hadAllPeerLoadedScene=true;
    }

    void Update() {
        _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount,_target,3*Time.deltaTime);
    }
}
