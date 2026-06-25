using UnityEngine;

public class SpectatorMode : MonoBehaviour {

    private bool enable;

    private string playerUUID;

    private TorsoIdentifier torso;

    void Start() {
        SpectatorModeManager.Instance.OnSpectatorModeActivation += SpectatorModeManager_OnSpectatorModeActivation;
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

    private void SpectatorModeManager_OnSpectatorModeActivation(object _sender,
    SpectatorModeManager.OnSpectatorModeActivationEventArgs _args) {
        //Case of another avatar being killed by the monster

        if (_args.PlayerUUID == playerUUID) {
            updateVisibility();
            return;
        }
    }

    void OnDestroy() {
        SpectatorModeManager.Instance.OnSpectatorModeActivation -= SpectatorModeManager_OnSpectatorModeActivation;
    }
}