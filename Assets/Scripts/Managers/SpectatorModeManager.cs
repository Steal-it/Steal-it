using System;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using System.Linq;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;

public class SpectatorModeManager : MonoBehaviour {
    public event EventHandler<OnSpectatorModeChangeEventArgs> OnSpectatorModeChanged;
    public class OnSpectatorModeChangeEventArgs : EventArgs {
        public string PlayerUUID;
    }

    public bool IsEnabled => isEnabled;

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
    [SerializeField, Tooltip("Maximum number of players allowed to die to continue to play. Set -1 for no limit (all players have to die for GameOver).")]
    private int maxDeadPlayersCount = -1;

    private bool isEnabled;
    private int deadPlayersCounter;

    void OnValidate() {
        if (maxDeadPlayersCount < -1) {
            maxDeadPlayersCount = -1;
        }
    }

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

    /// <summary>
    /// Main change handler: invoke when the monster made someone lose. Invoke with true if the message should be propagated. The only player that should invoke this function is the one attached to the monster, all of the other player will manage spectator mode upon receiving the appropriate message (that automatically won't propagate)
    /// </summary>
    public void ChangeSpectatorModeByPlayerUUID(string _playerUUID, bool _sendToOtherPeers) {
        // Ubiq does not guarantee the uuid will not change after connection/disconnection/room change, therefore, it is necessary to obtain it each time
        string playerUUID = NetworkReferenceManager.Instance.RoomClient.Me.uuid;

        // Update visibility for local player (XR rig) if he lost
        if (_playerUUID == playerUUID) {
            // Disable the oob collision detection
            blockPlayerVision.enabled = !blockPlayerVision.enabled;
            // Update visibility
            UpdateVisibility();
        }

        // Activate spectator mode for another peer (this will also invoke the RSOD if the local peer lost)
        OnSpectatorModeChanged?.Invoke(this, new OnSpectatorModeChangeEventArgs {
            PlayerUUID = _playerUUID
        });

        if (_sendToOtherPeers) {
            NetworkReferenceManager.Instance.MessageHandler.SendActivateSpectatorModeMessage(_playerUUID);
        }

        deadPlayersCounter++;

        int playersCount = NetworkReferenceManager.Instance.RoomClient.Peers.Count() + 1;
        if (
            (maxDeadPlayersCount == -1 && deadPlayersCounter == playersCount) ||
            (maxDeadPlayersCount != -1 && deadPlayersCounter > maxDeadPlayersCount)
        ) {
            // GameOver if too many players died
            if (isEnabled) {
                GameOver.Instance.Loser();
            } else {
                GameOver.Instance.Winner();
            }
        }
    }

    void OnDestroy() {
        NetworkReferenceManager.Instance.MessageHandler.OnApplySpectatorModeRequested -= MessageHandler_OnApplySpectatorModeRequest;
    }
}
