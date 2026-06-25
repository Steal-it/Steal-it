using System;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

public class SpectatorModeManager : MonoBehaviour {
    public static SpectatorModeManager Instance { get; private set; }
    public event EventHandler<OnSpectatorModeActivationEventArgs> OnSpectatorModeActivation;

    public class OnSpectatorModeActivationEventArgs : EventArgs {
        public string PlayerUUID;
    }
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
        // TODO ask And what to do to avoid monster recognition
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

    void Start() {
        NetworkReferenceManager.Instance.MessageHandler.OnApplySpectatorModeRequest += MessageHandler_OnApplySpectatorModeRequest;
    }

    void Awake() {
        if (Instance != null) {
            Debug.LogError($"An instance of {nameof(SpectatorModeManager)} already exists!");
            return;
        }

        Instance = this;
    }

    public void changeSpectatorModeByPlayerUUID() {
        // Change spectator mode for local avatar and send a message to all players connected to the room
        blockPlayerVision.enabled = !blockPlayerVision.enabled;

        updateVisibility();

        string playerUUID = NetworkReferenceManager.Instance.RoomClient.Me.uuid;

        //Send event: this will activate RSOD
        OnSpectatorModeActivation?.Invoke(this, new OnSpectatorModeActivationEventArgs {
            PlayerUUID = playerUUID
        });


        NetworkReferenceManager.Instance.MessageHandler.SendActivateSpectatorModeMessage(playerUUID);
    }

    void MessageHandler_OnApplySpectatorModeRequest(object _sender,
    MessageHandler.OnApplySpectatorModeRequestEventArgs _args) {
        // Activate spectator mode for another peer
        OnSpectatorModeActivation?.Invoke(this, new OnSpectatorModeActivationEventArgs {
            PlayerUUID = _args.PlayerUUID
        });
    }

    void OnDestroy() {
        NetworkReferenceManager.Instance.MessageHandler.OnApplySpectatorModeRequest -= MessageHandler_OnApplySpectatorModeRequest;
    }
}