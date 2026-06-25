using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

public class SpectatorMode : MonoBehaviour {

    public bool enable;

    void Start() {
        SpectatorModeManager.Instance.OnSpectatorModeActivation += SpectatorModeManager_OnSpectatorModeActivation;
    }
    void updateVisibility() {
        if (enable) {
            Enable();
        } else {
            Disable();
        }
    }

    private void Enable() {
        gameObject.SetActive(true);
    }

    private void Disable() {
        gameObject.SetActive(false);
    }

    private void SpectatorModeManager_OnSpectatorModeActivation(object _sender,
    SpectatorModeManager.OnSpectatorModeActivationEventArgs _args) {
        //Case of another avatar being killed by the monster
        string playerUUID = gameObject.name;
        if(_args.PlayerUUID == playerUUID) {
            updateVisibility();
            return;
        }
    }

    void OnDestroy() {
        SpectatorModeManager.Instance.OnSpectatorModeActivation -= SpectatorModeManager_OnSpectatorModeActivation;
    }
}