using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimator : AnimatorNetworkExtension {
    [SerializeField]
    private Animator animator;

    private const string CHASE_PARAM = "chase";
    private const string FLASHED_PARAM = "flashed";
    private const string MURDER_PARAM = "murder";
    private const string WANDER_PARAM = "wander";

    protected override void Awake() {
        base.Awake();
        Animator = animator;
        ParameterTypeDictionary = new Dictionary<string, IAnimationParameter>() {
            { CHASE_PARAM, new AnimationTriggerParameter() },
            { FLASHED_PARAM, new AnimationTriggerParameter() },
            { MURDER_PARAM, new AnimationTriggerParameter() },
            { WANDER_PARAM, new AnimationTriggerParameter() },
        };
    }

    public void SetChase() {
        ResetAllTriggers();
        animator.SetTrigger(CHASE_PARAM);

        NotifyParameterSet(CHASE_PARAM);
    }

    public void SetFlashed() {
        ResetAllTriggers();
        animator.SetTrigger(FLASHED_PARAM);

        NotifyParameterSet(FLASHED_PARAM);
    }

    public void SetMurder() {
        ResetAllTriggers();
        animator.SetTrigger(MURDER_PARAM);

        NotifyParameterSet(MURDER_PARAM);
    }

    public void SetWander() {
        ResetAllTriggers();
        animator.SetTrigger(WANDER_PARAM);

        NotifyParameterSet(WANDER_PARAM);
    }
}
