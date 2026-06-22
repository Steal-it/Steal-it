using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimator : AbstractNetworkAnimator {
    [SerializeField]
    private Animator animator;

    private const string IS_STUNNED_ANIM_VAR = "isStunned";

    void Awake() {
        Animator = animator;
        ParameterTypeDictionary = new Dictionary<string, IAnimationParameter>() {
            { IS_STUNNED_ANIM_VAR, new AnimationBoolParameter() }
        };
    }

    public void SetIsStunned(bool _value) {
        animator.SetBool(IS_STUNNED_ANIM_VAR, _value);

        NotifyParameterSet(IS_STUNNED_ANIM_VAR, _value.ToString());
    }
}
