using System;
using System.Collections;
using System.Collections.Generic;
using Ubiq;
using Ubiq.Dictionaries;
using Ubiq.Rooms;
using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(NetworkHandSide))]

public class AvatarHand : LocalAvatar {
    [SerializeField]
    private PlayerSettingsSO playerSettings;
    [SerializeField]
    private Side side;

    private HandAnimatorController animatorController;

    private NetworkHandSide networkHandSide;

    private static TorchLight leftTorchLight;
    private static TorchLight rightTorchLight;

    protected override void Awake() {
        base.Awake();
        TryGetComponent(out networkHandSide);
    }

    private void Start() {
        XROrigin origin = FindFirstObjectByType<XROrigin>();
        TorchAnimator torchAnimator = GetComponentInChildren<TorchAnimator>();
        animatorController = GetComponent<HandAnimatorController>();
        if (IsLocal) {
            if (side == Side.Left) {
                Transform leftHand = origin.transform.Find("Camera Offset/Left Controller");
                leftHand.GetComponentInChildren<HandCollisionController>().OnLadder += (_onLadder) => {
                    animatorController.ToggleHandStateAnimation(_onLadder);
                    animatorController.UpdateGripHand(side, _onLadder);
                };
                leftHand.GetComponentInChildren<HandCollisionController>().OnPoke += animatorController.CalculatePoke;
                leftHand.GetComponentInChildren<Torch>().OnTorchTurned += torchAnimator.ToggleLightVisual;
                leftTorchLight = leftHand.GetComponentInChildren<TorchLight>();
                torchAnimator.SetupTorch(leftHand.GetComponentInChildren<TorchLight>());
            } else {
                Transform rightHand = origin.transform.Find("Camera Offset/Right Controller");
                rightHand.GetComponentInChildren<HandCollisionController>().OnLadder += (_onLadder) => {
                    animatorController.ToggleHandStateAnimation(_onLadder);
                    animatorController.UpdateGripHand(side, _onLadder);
                };
                rightHand.GetComponentInChildren<HandCollisionController>().OnPoke += animatorController.CalculatePoke;
                rightHand.GetComponentInChildren<Torch>().OnTorchTurned += torchAnimator.ToggleLightVisual;
                rightTorchLight = rightHand.GetComponentInChildren<TorchLight>();
                torchAnimator.SetupTorch(rightHand.GetComponentInChildren<TorchLight>());
            }
            NetworkReferenceManager.Instance.LevelManager.OnGameLoaded += UpdateHandsVisual;
        } else {
            if (side == Side.Left) {
                Transform leftHand = origin.transform.Find("Camera Offset/Left Controller");
                torchAnimator.SetupTorch(leftTorchLight);
            } else {
                Transform rightHand = origin.transform.Find("Camera Offset/Right Controller");
                torchAnimator.SetupTorch(rightTorchLight);
            }
            networkHandSide.OnMessageReceived += ChangeHandTorch;
        }

        animatorController.ToggleHandStateAnimation(true);
        animatorController.UpdateGripHand(side, true);
    }

    private void UpdateHandsVisual(object _, LevelManager.OnGameLoadedEventArgs __) {
        ChangeHandTorch(playerSettings.playerTorchHand);
        networkHandSide.SendSideParameters(playerSettings.playerTorchHand, NetworkReferenceManager.Instance.RoomClient.Me.uuid);
    }

    private void ChangeHandTorch(Side _side) {
        bool amITheTorchHand = side == _side;
        animatorController.ToggleHandStateAnimation(!amITheTorchHand);
        animatorController.UpdateGripHand(side, !amITheTorchHand);
    }
}
