using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(NetworkAnimation))]
public class FreeHandAnimator : AnimatorNetworkExtension {
    [SerializeField]
    private Animator animator;
    [SerializeField]
    [Tooltip("Speed at which the hand model will change grip strength in units/second. A speed of 2 will change from 0 (no grip) to 1 (full grip) in 0.5 seconds, for example. Set to 0 to disable smoothing")]
    private float smoothingSpeed = 4;
    [SerializeField]
    private float pokeAnimationOffset;

    private NetworkAnimation networkAnimation;

    public float TargetGrip { private get; set; }
    public float TargetPoke { private get; set; }
    private float currentGrip;
    private float currentPoke;
    private bool completeGrip;
    private bool torch = false;

    private const string GRIP = "Grabbing";
    private const string POKE = "Pokeing";
    private const string COMPLETED = "CompletedGrab";
    private const string TORCH = "InTorchPosition";

    protected override void Awake() {
        base.Awake();
        Animator = animator;
        ParameterTypeDictionary = new Dictionary<string, IAnimationParameter>() {
            { GRIP, new AnimationFloatParameter() },
            { POKE, new AnimationFloatParameter() },
            { COMPLETED, new AnimationBoolParameter() },
            { TORCH, new AnimationBoolParameter() },
        };
        TryGetComponent(out networkAnimation);
    }

    void Start() {
        if (IsLocal) {
            OnAnimationChanged += SendAnimation;
        } else {
            networkAnimation.OnMessageReceived += ReciveAnimation;
        }
    }

    private void SendAnimation(object sender, OnAnimationChangedEventArgs _event) {
        networkAnimation.SendAnimationParameters(_event.ParameterDictionary);
    }

    private void ReciveAnimation(object sender, NetworkAnimation.OnMessageReceivedEventArgs _event) {
        SetParameterDictionary(_event.ParameterDictionary);
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
        networkAnimation.OnMessageReceived -= ReciveAnimation;
    }

}
