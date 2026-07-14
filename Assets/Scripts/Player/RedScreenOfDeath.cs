using UnityEngine;

public class RedScreenOfDeath : MonoBehaviour {
    [SerializeField]
    private float uiDistance;

    private bool enable;
    private Canvas canvas;

    void Start() {
        NetworkReferenceManager.Instance.SpectatorModeManager.OnSpectatorModeChanged += SpectatorModeManager_OnSpectatorModeChange;

        if (transform.GetChild(0).TryGetComponent(out canvas)) {
            if (Camera.main == null) {
                Debug.LogWarning("Main camera not found");
            }

            canvas.worldCamera = Camera.main;
            canvas.planeDistance = uiDistance;
            canvas.gameObject.SetActive(false);
        }
    }

    private void SpectatorModeManager_OnSpectatorModeChange(object _sender, SpectatorModeManager.OnSpectatorModeChangeEventArgs _event) {
        // Ubiq does not guarantee the uuid will not change after connection/disconnection/room change, therefore, it is necessary to obtain it each time
        string playerUUID = NetworkReferenceManager.Instance.RoomClient.Me.uuid;

        //The event is invoked both when another peer lost or another peer lost. However, the screen should be activated only if this local peer lost
        if (_event.PlayerUUID == playerUUID) {
            enable = !enable;
            canvas.gameObject.SetActive(enable);
        }
    }

    void OnDestroy() {
        NetworkReferenceManager.Instance.SpectatorModeManager.OnSpectatorModeChanged -= SpectatorModeManager_OnSpectatorModeChange;
    }
}