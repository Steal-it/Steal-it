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
    private CharacterController characterController;
    [SerializeField]
    private GravityProvider gravityProvider;
    [SerializeField, Range(2, 4)]
    private float height = 3;

    public bool enable;

    void Update() {
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

        characterController.enabled = false;
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
        Debug.Log("Invoked");
        Update();

        string playerUUID = NetworkReferenceManager.Instance.RoomClient.Me.uuid;
        NetworkReferenceManager.Instance.MessageHandler.SendActivateSpectatorModeMessage(playerUUID);
    }

    void MessageHandler_OnApplySpectatorModeRequest(object _sender,
    MessageHandler.OnApplySpectatorModeRequestEventArgs _args) {
        // Activate spectator mode locally ONLY
        OnSpectatorModeActivation?.Invoke(this, new OnSpectatorModeActivationEventArgs {
           PlayerUUID=_args.PlayerUUID
        });
    }
}