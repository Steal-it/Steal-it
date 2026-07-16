using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using static MessageHandler;

public class FreeHandAnimator : AnimatorNetworkExtension {
    [SerializeField]
    private Animator animator;
    [SerializeField]
    [Tooltip("Speed at which the hand model will change grip strength in units/second. A speed of 2 will change from 0 (no grip) to 1 (full grip) in 0.5 seconds, for example. Set to 0 to disable smoothing")]
    private float smoothingSpeed = 4;
    [SerializeField]
    private float pokeAnimationOffset;
    public float TargetGrip { private get; set; }
    public float TargetPoke { private get; set; }
    private float currentGrip;
    private float currentPoke;
    private bool completeGrip;
    private bool torch = false;
    private Transform parent;

    private const string GRIP = "Grabbing";
    private const string POKE = "Pokeing";
    private const string COMPLETED = "CompletedGrab";
    private const string TORCH = "InTorchPosition";

    protected override void Awake() {
        base.Awake();
        Animator = animator;
        ParameterTypeDictionary = new Dictionary<string, IAnimationParameter>() {
            { POKE, new AnimationFloatParameter() },
        };
    }

    void Start() {
        if (IsLocal) {
            OnAnimationChanged += SendAnimation;
        } else {
            NetworkReferenceManager.Instance.MessageHandler.OnAvatarAnimationMessageReceived += ReceiveAnimation;
        }
    }

    private void SendAnimation(object sender, OnAnimationChangedEventArgs _event) {
        NetworkReferenceManager.Instance.MessageHandler.SendAvatarAnimationMessage(_event.ParameterDictionary);
    }

    private void ReceiveAnimation(object sender, OnAvatarAnimationMessageReceivedEventArgs _args) {
        if (parent == null) {
            parent = transform;
        }

        string playerUUID = parent.name;

        while (!playerUUID.Contains("Remote Avatar") && !playerUUID.Contains("Local Avatar")) {
            parent = parent.parent;
            playerUUID = parent.name;
        }

        if (playerUUID != "Local Avatar") {
            playerUUID = playerUUID.Split('#')[1];
            if (_args.PlayerUUID == playerUUID) {
                SetParameterDictionary(_args.ParameterDictionary);
            }
        }

    }

    private void Update() {
        if (torch) return;

        if (!Mathf.Approximately(currentGrip, TargetGrip)) {
            var delta = smoothingSpeed * Time.deltaTime;
            currentGrip = Mathf.MoveTowards(currentGrip, TargetGrip, delta);

            if (currentGrip == 1) {
                completeGrip = true;
                animator.SetBool(COMPLETED, completeGrip);
            }
            if (completeGrip && currentGrip < 0.9f) {
                completeGrip = false;
                animator.SetBool(COMPLETED, completeGrip);
            }
            animator.SetFloat(GRIP, currentGrip);

        }
        if (!Mathf.Approximately(currentPoke, TargetPoke)) {
            if (IsLocal) {
                var delta = smoothingSpeed * Time.deltaTime;
                currentPoke = Mathf.MoveTowards(currentPoke, TargetPoke, delta);
                animator.SetFloat(POKE, currentPoke);
                NotifyParameterSet(POKE, currentPoke.ToString());
            }
        }
    }


    public void ToggleTorchPosition(bool _value) {
        animator.SetBool(TORCH, _value);
        torch = _value;
    }

    void OnDestroy() {
        NetworkReferenceManager.Instance.MessageHandler.OnAvatarAnimationMessageReceived -= ReceiveAnimation;
    }

}
