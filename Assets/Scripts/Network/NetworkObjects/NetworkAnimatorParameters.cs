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

public class AnimationIntParameter : IAnimationParameter {
    public bool TrySet(string _name, string _value, Animator _animator) {
        bool result = int.TryParse(_value, out int parsedValue);

        if (result) {
            _animator.SetInteger(_name, parsedValue);
        }

        return result;
    }
}

public class AnimationFloatParameter : IAnimationParameter {
    public bool TrySet(string _name, string _value, Animator _animator) {
        bool result = float.TryParse(_value, out float parsedValue);

        if (result) {
            _animator.SetFloat(_name, parsedValue);
        }

        return result;
    }
}

public class AnimationTriggerParameter : IAnimationParameter {
    public bool TrySet(string _name, string _value, Animator _animator) {
        _animator.SetTrigger(_name);

        return true;
    }
}
