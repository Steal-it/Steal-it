using System;
using UnityEngine;

public class MonsterAnimator : AbstractNetworkAnimator {
    public event EventHandler<OnAnimationChangedEventArgs> OnAnimationChanged;
    public class OnAnimationChangedEventArgs : EventArgs {
        public bool IsStunned;
    }

    [SerializeField]
    private Animator animator;

    private const string IS_STUNNED_ANIM_VAR = "isStunned";

    protected override void OnParametersSet() {
        if (TryGetBool(IS_STUNNED_ANIM_VAR, out bool value)) {
            SetIsStunned(value);
        }
    }

    public void SetIsStunned(bool _value) {
        animator.SetBool(IS_STUNNED_ANIM_VAR, _value);

        OnAnimationChanged?.Invoke(this, new OnAnimationChangedEventArgs {
            IsStunned = _value
        });
    }
}
