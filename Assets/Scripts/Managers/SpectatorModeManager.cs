using System;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;

public class SpectatorModeManager : MonoBehaviour {
    public event EventHandler<OnSpectatorModeChangeEventArgs> OnSpectatorModeChange;

    public class OnSpectatorModeChangeEventArgs : EventArgs {
        public string PlayerUUID;
    }
    [SerializeField]
    private XROrigin rig;
    [SerializeField]
    private BlockPlayerVision blockPlayerVision;
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private List<XRInteractionGroup> controllers;
    [SerializeField]
    private GravityProvider gravityProvider;
    [SerializeField, Range(0, 4)]
    private float height = 0;

    private bool enable;

    void updateVisibility() {
        enable = !enable;
        if (enable) {
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

        controllers.ForEach((controller) => {
            controller.gameObject.SetActive(false);
        });
    }

    private void Disable() {
        Vector3 position = rig.transform.position;
        position.y = 0;
        rig.transform.position = position;

        characterController.enabled = true;
        gravityProvider.enabled = true;

        controllers.ForEach((controller) => {
            controller.gameObject.SetActive(true);
        });
    }

    void Start() {
        NetworkReferenceManager.Instance.MessageHandler.OnApplySpectatorModeRequest += MessageHandler_OnApplySpectatorModeRequest;
    }

    void Awake() {
        enable = false;
    }

    // Main change handler: invoke when the monster made someone lose. Invoke with true if the message should be propagated. The only player that should invoke this function is the one attached to the monster, all of the other player will manage spectator mode upon receiving the appropriate message (that automatically won't propagate)
    public void ChangeSpectatorModeByPlayerUUID(string _playerUUID, bool _sendToOtherPeer) {
        // Ubiq does not guarantee the uuid will not change after connection/disconnection/room change, therefore, it is necessary to obtain it each time
        string playerUUID = NetworkReferenceManager.Instance.RoomClient.Me.uuid;

        //The event is invoked both when another peer lost or another peer lost. However, locally the spectator mode should be activated only if this local peer lost
        if (_playerUUID == playerUUID) {
            // Disable the oob collision detection
            blockPlayerVision.enabled = !blockPlayerVision.enabled;
            // Update visibility
            updateVisibility();
        }

        // Activate spectator mode for another peer (this will also invoked the RSOD if the local peer lost)
        OnSpectatorModeChange?.Invoke(this, new OnSpectatorModeChangeEventArgs {
            PlayerUUID = _playerUUID
        });

        if (_sendToOtherPeer) {
            NetworkReferenceManager.Instance.MessageHandler.SendActivateSpectatorModeMessage(_playerUUID);
        }
    }

    // Invoked when the local peer receives a message that requires the spectator mode of someone to be activated
    void MessageHandler_OnApplySpectatorModeRequest(object _sender,
    MessageHandler.OnApplySpectatorModeRequestEventArgs _args) {
        ChangeSpectatorModeByPlayerUUID(_args.PlayerUUID, false);
    }

    void OnDestroy() {
        NetworkReferenceManager.Instance.MessageHandler.OnApplySpectatorModeRequest -= MessageHandler_OnApplySpectatorModeRequest;
    }
}