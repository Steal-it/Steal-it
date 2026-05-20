using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

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
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } 
        else
        {
            Destroy(gameObject);
        }
    }
    
    public async void LoadScreen(string sceneName)
    {
        _target = 0;
        _progressBar.fillAmount = 0;

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        _loaderCanvas.SetActive(true);
        hadAllPeerLoadedScene = false;

        do {
            await Task.Delay(100);

            _target = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(1000);
        if(loadingInfoText) {
            loadingInfoText.text = "Waiting other players to finish loading...";
        }
        msgHandler.SendLoadSceneCompletedMessage();

        do {
            await Task.Delay(100);
        } while(!hadAllPeerLoadedScene);

        hadAllPeerLoadedScene = false;
        scene.allowSceneActivation = true;
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
