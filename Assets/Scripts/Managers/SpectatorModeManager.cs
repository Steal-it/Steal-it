using System;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

public class SpectatorModeManager : MonoBehaviour {
    public event EventHandler<OnSpectatorModeChangeEventArgs> OnSpectatorModeChanged;
    public class OnSpectatorModeChangeEventArgs : EventArgs {
        public string PlayerUUID;
    }

    public bool IsEnabled => isEnabled;
    public int DeadPlayersCounter => deadPlayersCounter;

    [SerializeField]
    private XROrigin rig;
    [SerializeField]
    private BlockPlayerVision blockPlayerVision;
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private GravityProvider gravityProvider;
    [SerializeField, Range(0, 4)]
    private float height = 0;

    private bool isEnabled;
    private int deadPlayersCounter;

    void Start() {
        NetworkReferenceManager.Instance.MessageHandler.OnApplySpectatorModeRequested += MessageHandler_OnApplySpectatorModeRequest;
    }

    /// <summary>
    /// Invoked when the local peer receives a message that requires the spectator mode of someone to be activated
    /// </summary>
    void MessageHandler_OnApplySpectatorModeRequest(object _sender, MessageHandler.OnApplySpectatorModeRequestEventArgs _event) {
        ChangeSpectatorModeByPlayerUUID(_event.PlayerUUID, false);
    }

    private void UpdateVisibility() {
        isEnabled = !isEnabled;
        if (isEnabled) {
            Enable();
        } else {
            Disable();
        }
    }

    private void Enable() {
        Vector3 position = rig.transform.position;
        position.y = height;
        rig.transform.position = position;

        characterController.enabled = false; // Do not collide
        gravityProvider.enabled = false;
    }

    private void Disable() {
        Vector3 position = rig.transform.position;
        position.y = 0;
        rig.transform.position = position;

        characterController.enabled = true;
        gravityProvider.enabled = true;
    }

    /// <summary>
    /// Main change handler: invoke when the monster made someone lose. Invoke with true if the message should be propagated. The only player that should invoke this function is the one attached to the monster, all of the other player will manage spectator mode upon receiving the appropriate message (that automatically won't propagate)
    /// </summary>
    public void ChangeSpectatorModeByPlayerUUID(string _playerUUID, bool _sendToOtherPeers) {
        // Ubiq does not guarantee the uuid will not change after connection/disconnection/room change, therefore, it is necessary to obtain it each time
        string playerUUID = NetworkReferenceManager.Instance.RoomClient.Me.uuid;

        // The event is invoked both when another peer lost or another peer lost. However, locally the spectator mode should be activated only if this local peer lost
        if (_playerUUID == playerUUID) {
            // Disable the oob collision detection
            blockPlayerVision.enabled = !blockPlayerVision.enabled;
            // Update visibility
            UpdateVisibility();
        }

        deadPlayersCounter++;

        // Activate spectator mode for another peer (this will also invoked the RSOD if the local peer lost)
        OnSpectatorModeChanged?.Invoke(this, new OnSpectatorModeChangeEventArgs {
            PlayerUUID = _playerUUID
        });

        if (_sendToOtherPeers) {
            NetworkReferenceManager.Instance.MessageHandler.SendActivateSpectatorModeMessage(_playerUUID);
        }
    }

    void OnDestroy() {
        NetworkReferenceManager.Instance.MessageHandler.OnApplySpectatorModeRequested -= MessageHandler_OnApplySpectatorModeRequest;
    }
}