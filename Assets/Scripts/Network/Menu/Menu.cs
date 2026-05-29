using UnityEngine;
using System;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    [SerializeField]
    private Button readyButton;

    private MsgHandler msgHandler;

    void Start() {
        msgHandler = NetworkReferenceManager.Instance.MsgHandler;

        msgHandler.OnCounterRecoverFinished += OnCounterRecoverFinishedHandler;
        readyButton.onClick.AddListener(msgHandler.SendReadyMessage);
    }

    private void OnCounterRecoverFinishedHandler(object _sender, EventArgs _e) {
        readyButton.gameObject.SetActive(true);
    }

    void OnDestroy() {
        msgHandler.OnCounterRecoverFinished -= OnCounterRecoverFinishedHandler;
        readyButton.onClick.RemoveAllListeners();
    }
}
