using System;
using System.Collections.Generic;
using Ubiq.Dictionaries;
using UnityEngine;

public class MonsterAnimator : AbstractNetworkAnimator {
    public event EventHandler<OnAnimationChangedEventArgs> OnAnimationChanged;
    public class OnAnimationChangedEventArgs : EventArgs {
        public SerializableDictionary ParameterDictionary;
        // public Dictionary<string, bool> ParameterDictionary;
    }

    [SerializeField]
    private Animator animator;

    private const string IS_STUNNED_ANIM_VAR = "isStunned";

    protected override void OnParametersSet() {
        // if (TryGetBool(IS_STUNNED_ANIM_VAR, out AnimationMessage.AnimatorBoolParameter value)) {
        if (TryGetBool(IS_STUNNED_ANIM_VAR, out bool value)) {
            SetIsStunned(value);
        }
    }

    public void SetIsStunned(bool _value) {
        animator.SetBool(IS_STUNNED_ANIM_VAR, _value);

        SerializableDictionary d = new SerializableDictionary();
        d.Update(IS_STUNNED_ANIM_VAR, _value.ToString());

        OnAnimationChanged?.Invoke(this, new OnAnimationChangedEventArgs {
            ParameterDictionary = d
            // ParameterDictionary = new SerializableDictionary {
            //         new KeyValuePair
            //         { IS_STUNNED_ANIM_VAR, new AnimatorBoolParameter(_value) }
            //     }
            // ParameterDictionary = new Dictionary<string, bool>() {
            //     { IS_STUNNED_ANIM_VAR, _value }
            // }
        });
    }
}
