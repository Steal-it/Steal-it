using UnityEngine;
using System;

public class Menu : MonoBehaviour {
    [SerializeField]
    private GameObject readyButton;
    [SerializeField]
    private Transform spawnRelativeTransform;

    private MsgHandler msgHandler;

    void OnEnable() {
        msgHandler = MsgHandler.Instance;

        msgHandler.OnCounterRecoverFinished += OnCounterRecoverFinishedHandler;
    }

    public void Request() {
        var cam = Camera.main.transform;
        transform.position = cam.TransformPoint(spawnRelativeTransform.localPosition);
        transform.rotation = cam.rotation * spawnRelativeTransform.localRotation;
        gameObject.SetActive(true);
    }

    private void OnCounterRecoverFinishedHandler(object _sender, EventArgs _e) {
        readyButton.SetActive(true);
    }

    void OnDestroy() {
        msgHandler.OnCounterRecoverFinished -= OnCounterRecoverFinishedHandler;
    }
}
