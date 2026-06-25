using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

public class SpectatorMode : MonoBehaviour {

    private bool enable;

    void Start() {
        SpectatorModeManager.Instance.OnSpectatorModeActivation += SpectatorModeManager_OnSpectatorModeActivation;
    }
    void updateVisibility() {
        enable=!enable;
        if (enable) {
            Enable();
        } else {
            Disable();
        }
    }

    private void Enable() {
        gameObject.SetActive(false);
    }

    private void Disable() {
        gameObject.SetActive(true);
    }

    private void SpectatorModeManager_OnSpectatorModeActivation(object _sender,
    SpectatorModeManager.OnSpectatorModeActivationEventArgs _args) {
        //Case of another avatar being killed by the monster
        string playerUUID = gameObject.name;

        if(playerUUID == "Local Avatar") {
            return;
        }

        playerUUID = playerUUID.Split('#')[1];
        if(_args.PlayerUUID == playerUUID) {
            updateVisibility();
            return;
        }
    }

    void OnDestroy() {
        SpectatorModeManager.Instance.OnSpectatorModeActivation -= SpectatorModeManager_OnSpectatorModeActivation;
    }
}