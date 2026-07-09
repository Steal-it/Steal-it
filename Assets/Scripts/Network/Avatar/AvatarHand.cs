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
            playerSettings.OnPlayerTorchChanged.Register(ChangeHandTorch);
            // FreeHand();
            ChangeHandTorch(playerSettings.playerTorchHand);
            NetworkReferenceManager.Instance.RoomClient.OnPeerAdded.AddListener(SendSidePeer);
            // NetworkReferenceManager.Instance.LevelManager.OnGameLoaded += UpdateHandsVisual;
        } else {
            // FreeHand();
            if (side == Side.Left) {
                Transform leftHand = origin.transform.Find("Camera Offset/Left Controller");
                torchAnimator.SetupTorch(leftTorchLight);
            } else {
                Transform rightHand = origin.transform.Find("Camera Offset/Right Controller");
                torchAnimator.SetupTorch(rightTorchLight);
            }
            networkHandSide.OnMessageReceived += ChangeHandTorch;
        }
    }

    private void FreeHand() {
        animatorController.ToggleHandStateAnimation(true);
        animatorController.UpdateGripHand(side, true);
    }

    private void UpdateHandsVisual(object _, LevelManager.OnGameLoadedEventArgs __) {
        ChangeHandTorch(playerSettings.playerTorchHand);
        networkHandSide.SendSideParameters(playerSettings.playerTorchHand);
        // StartCoroutine(SendDelayedMessageCoroutine());
    }

    private void SendSidePeer(IPeer _) {
        StartCoroutine(SendDelayedMessageCoroutine());
    }
    private IEnumerator SendDelayedMessageCoroutine() {
        // Wait for the end of the frame or a couple of frames 
        // to guarantee the joining client has run Awake/Start/Register
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);

        networkHandSide.SendSideParameters(playerSettings.playerTorchHand);
    }

    private void ChangeHandTorch(Side _side) {
        if (IsLocal) {
            networkHandSide.SendSideParameters(_side);
        }
        bool amITheTorchHand = side == _side;
        animatorController.ToggleHandStateAnimation(!amITheTorchHand);
        animatorController.UpdateGripHand(side, !amITheTorchHand);
    }

    void OnDestroy() {
        playerSettings.OnPlayerTorchChanged.Unregister(ChangeHandTorch);
        NetworkReferenceManager.Instance.RoomClient.OnPeerAdded.RemoveListener(SendSidePeer);
    }
}
