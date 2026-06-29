using UnityEngine;

public class RedScreenOfDeath : MonoBehaviour {

    [SerializeField]
    private float uiDistance;
    private bool enable;
    private GameObject canvas;
    void Start() {
        enable = false;
        canvas = gameObject.transform.GetChild(0).gameObject;
        canvas.TryGetComponent<Canvas>(out var localCanvas);

        if (Camera.main == null) {
            Debug.LogWarning("Main camera not found");
        }

        localCanvas.planeDistance = uiDistance;

        SpectatorModeManager.Instance.OnSpectatorModeChange += SpectatorModeManager_OnSpectatorModeChange;
    }

    private void SpectatorModeManager_OnSpectatorModeChange(object _sender,
    SpectatorModeManager.OnSpectatorModeChangeEventArgs _args) {
        enable = !enable;

        // Ubiq does not guarantee the uuid will not change after connection/disconnection/room change, therefore, it is necessary to obtain it each time
        string playerUUID = NetworkReferenceManager.Instance.RoomClient.Me.uuid;

        //The event is invoked both when another peer lost or another peer lost. However, the screen should be activated only if this local peer lost
        if (_args.PlayerUUID == playerUUID || _args.PlayerUUID == "Local Avatar") {
            canvas.SetActive(enable);
        } else {
            Debug.Log("No RSOD " + _args.PlayerUUID + " " + playerUUID);
        }
    }

    void OnDestroy() {
        SpectatorModeManager.Instance.OnSpectatorModeChange -= SpectatorModeManager_OnSpectatorModeChange;
    }
}