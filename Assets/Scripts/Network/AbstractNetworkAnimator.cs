using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractNetworkAnimator : MonoBehaviour {
    private Dictionary<string, AnimationMessage.AnimatorParameter> parameterDictionary;
    // private Dictionary<string, bool> parameterDictionary;

    // protected bool TryGetBool(string _name, out AnimationMessage.AnimatorBoolParameter _value) {
    protected bool TryGetBool(string _name, out bool _value) {
        bool result = parameterDictionary.TryGetValue(_name, out AnimationMessage.AnimatorParameter value);
        print("anim: " + parameterDictionary.Count);
        print(result);
        print(value);

        if (result) {
            _value = ((AnimationMessage.AnimatorBoolParameter)value).Value;
            // _value = value;
            return true;
        }

        _value = false;
        return false;
    }

    protected abstract void OnParametersSet();

    public void SetParameterDictionary(Dictionary<string, AnimationMessage.AnimatorParameter> _parameterDictionary) {
        // public void SetParameterDictionary(Dictionary<string, bool> _parameterDictionary) {
        parameterDictionary = _parameterDictionary;

        OnParametersSet();
    }
}