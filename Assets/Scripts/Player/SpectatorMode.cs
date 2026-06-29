using UnityEngine;

public class SpectatorMode : MonoBehaviour {

    private bool enable;

    private string playerUUID;

    private TorsoIdentifier torso;

    void Start() {
        SpectatorModeManager.Instance.OnSpectatorModeChange += SpectatorModeManager_OnSpectatorModeChange;
        playerUUID = gameObject.name;
        if (playerUUID != "Local Avatar") {
            playerUUID = playerUUID.Split('#')[1];
        }

        torso = gameObject.GetComponentInChildren<TorsoIdentifier>();
    }

    void updateVisibility() {
        enable = !enable;
        if (enable) {
            Enable();
        } else {
            Disable();
        }
    }

    private void Enable() {
        gameObject.SetActive(false);

        //Disable the collider
        if (torso) {
            var col = torso.GetComponent<Collider>();
            col.enabled = false;
        }
    }

    private void Disable() {
        gameObject.SetActive(true);
        if (torso) {
            var col = torso.GetComponent<Collider>();
            col.enabled = true;
        }
    }

    private void SpectatorModeManager_OnSpectatorModeChange(object _sender,
    SpectatorModeManager.OnSpectatorModeChangeEventArgs _args) {
        //Case of another avatar being killed by the monster

        if (_args.PlayerUUID == playerUUID || _args.PlayerUUID == NetworkReferenceManager.Instance.RoomClient.Me.uuid) {
            updateVisibility();
            return;
        }
    }

    void OnDestroy() {
        SpectatorModeManager.Instance.OnSpectatorModeChange -= SpectatorModeManager_OnSpectatorModeChange;
    }
}