using System.Collections.Generic;
using UnityEngine;

public interface IAnimationParameter {
    bool TrySet(string _name, string _value, Animator _animator);
}

public class AnimationBoolParameter : IAnimationParameter {
    public bool TrySet(string _name, string _value, Animator _animator) {
        bool result = bool.TryParse(_value, out bool parsedValue);

        if (result) {
            _animator.SetBool(_name, parsedValue);
        }

        return result;
    }
}

public class MonsterAnimator : AbstractNetworkAnimator {
    // public event EventHandler<OnAnimationChangedEventArgs> OnAnimationChanged;
    // public class OnAnimationChangedEventArgs : EventArgs {
    //     public SerializableDictionary ParameterDictionary;
    // }

    [SerializeField]
    private Animator animator;

    private const string IS_STUNNED_ANIM_VAR = "isStunned";
    // private Dictionary<string, IAnimationParameter> parameterTypeDictionary;

    void Awake() {
        Animator = animator;
        ParameterTypeDictionary = new Dictionary<string, IAnimationParameter>() {
            { IS_STUNNED_ANIM_VAR, new AnimationBoolParameter() }
        };
    }

    // private void NotifyParameterSet(string _name, string _value) {
    //     SerializableDictionary parameterDictionary = new SerializableDictionary();
    //     parameterDictionary.Update(_name, _value.ToString());

    //     OnAnimationChanged?.Invoke(this, new OnAnimationChangedEventArgs {
    //         ParameterDictionary = parameterDictionary
    //     });
    // }

    public void SetIsStunned(bool _value) {
        animator.SetBool(IS_STUNNED_ANIM_VAR, _value);

        NotifyParameterSet(IS_STUNNED_ANIM_VAR, _value.ToString());
    }

    // public void SetParameterDictionary(SerializableDictionary _parameterDictionary) {
    //     foreach (KeyValuePair<string, string> entry in _parameterDictionary) {
    //         if (parameterTypeDictionary.ContainsKey(entry.Key)) {
    //             parameterTypeDictionary[entry.Key].TrySet(entry.Key, entry.Value, animator);
    //         }
    //     }
    // }
}
