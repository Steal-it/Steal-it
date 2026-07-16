using System;
using Unity.XR.CoreUtils;
using UnityEngine;

public class AvatarHand : LocalAvatar {
    [SerializeField]
    private PlayerSettingsSO playerSettings;
    [SerializeField]
    private Side side;

    private HandAnimatorController animatorController;
    private TorchAnimator torchAnimator;

    protected override void Awake() {
        base.Awake();
    }

    private void Start() {
        // XROrigin origin = FindFirstObjectByType<XROrigin>();
        torchAnimator = GetComponentInChildren<TorchAnimator>();
        animatorController = GetComponent<HandAnimatorController>();

        ChangeHandTorch(false);
        if (IsLocal) {
            NetworkReferenceManager.Instance.LevelManager.OnGameLoaded += UpdateHandsVisual;
        } else {
            NetworkReferenceManager.Instance.MessageHandler.OnAvatarSendHandSideMessageReceived += OnAvatarSendHandMessageMessageReceived;
        }

    }

    private void OnGripRecived(object sender, MessageHandler.OnAvatarComponentEnablerMessageReceivedEventArgs _args) {
        animatorController.OnGripReceived(side, sender, _args);
    }

    private void UpdateHandsVisual(object _, LevelManager.OnGameLoadedEventArgs __) {
        XROrigin origin = FindFirstObjectByType<XROrigin>();
        Transform leftHand = origin.transform.Find("Camera Offset/Left Controller");
        Transform rightHand = origin.transform.Find("Camera Offset/Right Controller");

        if (playerSettings.playerTorchHand == side) { // if i am the torchhand
            ChangeHandTorch(true);
            print(torchAnimator);
            if (side == Side.Left) {
                leftHand.GetComponentInChildren<HandCollisionController>().OnLadder += (_onLadder) => {
                    animatorController.UpdateGripHand(side, _onLadder);
                    NetworkReferenceManager.Instance.MessageHandler.SendAvatarComponentEnablerMessage(AvatarComponentType.GripHand, _onLadder);
                };
                leftHand.GetComponentInChildren<Torch>().OnTorchTurned += torchAnimator.ToggleLightVisual;
                torchAnimator.OnTorchPocket += leftHand.GetComponentInChildren<Torch>().ToggleInPocket;
                torchAnimator.SetupTorch(leftHand.GetComponentInChildren<TorchLight>());
            } else {
                rightHand.GetComponentInChildren<HandCollisionController>().OnLadder += (_onLadder) => {
                    animatorController.UpdateGripHand(side, _onLadder);
                    NetworkReferenceManager.Instance.MessageHandler.SendAvatarComponentEnablerMessage(AvatarComponentType.GripHand, _onLadder);
                };
                rightHand.GetComponentInChildren<Torch>().OnTorchTurned += torchAnimator.ToggleLightVisual;
                torchAnimator.OnTorchPocket += rightHand.GetComponentInChildren<Torch>().ToggleInPocket;
                torchAnimator.SetupTorch(rightHand.GetComponentInChildren<TorchLight>());
            }

        } else { // im not the torchhand
            ChangeHandTorch(false);
            if (side == Side.Left) {
                leftHand.GetComponentInChildren<HandCollisionController>().OnPoke += animatorController.CalculatePoke;
                leftHand.GetComponentInChildren<Hand>().ChangeHandTorch(false);
                rightHand.GetComponentInChildren<Hand>().ChangeHandTorch(true);
            } else {
                rightHand.GetComponentInChildren<HandCollisionController>().OnPoke += animatorController.CalculatePoke;
                rightHand.GetComponentInChildren<Hand>().ChangeHandTorch(false);
                leftHand.GetComponentInChildren<Hand>().ChangeHandTorch(true);
            }
        }
        NetworkReferenceManager.Instance.MessageHandler.SendAvatarSendHandSideMessage(playerSettings.playerTorchHand);
    }

    private void OnAvatarSendHandMessageMessageReceived(object _sender, MessageHandler.OnAvatarSendHandSideMessageReceivedEventArgs _args) {
        string playerUUID = transform.parent.parent.name;
        if (playerUUID != "Local Avatar") {
            playerUUID = playerUUID.Split('#')[1];
            if (_args.PlayerUUID == playerUUID) {
                XROrigin origin = FindFirstObjectByType<XROrigin>();
                ChangeHandTorch(_args.Side == side);
                if (_args.Side == side) { // if i am the torchhand
                    NetworkReferenceManager.Instance.MessageHandler.OnAvatarComponentEnablerMessageReceived += OnGripRecived;
                    NetworkReferenceManager.Instance.MessageHandler.OnAvatarComponentEnablerMessageReceived += torchAnimator.OnAvatarComponentEnablerMessageReceived;
                    if (side == Side.Left) {
                        Transform leftHand = origin.transform.Find("Camera Offset/Left Controller");
                        torchAnimator.SetupTorch(leftHand.GetComponentInChildren<TorchLight>());
                        torchAnimator.OnTorchPocket += leftHand.GetComponentInChildren<Torch>().ToggleInPocket;
                    } else {
                        Transform rightHand = origin.transform.Find("Camera Offset/Right Controller");
                        torchAnimator.SetupTorch(rightHand.GetComponentInChildren<TorchLight>());
                        torchAnimator.OnTorchPocket += rightHand.GetComponentInChildren<Torch>().ToggleInPocket;
                    }
                }
                // im not the torchhand
                // do nothing special
            }
        }
    }

    private void ChangeHandTorch(bool amITheTorchHand) {
        animatorController.UpdateGripHand(side, !amITheTorchHand);
    }
}
