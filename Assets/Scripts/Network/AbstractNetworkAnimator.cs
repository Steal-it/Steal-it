using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractNetworkAnimator : MonoBehaviour {
    private Dictionary<string, AnimatorParameter> parameterDictionary;
    // private Dictionary<string, bool> parameterDictionary;

    // protected bool TryGetBool(string _name, out AnimationMessage.AnimatorBoolParameter _value) {
    protected bool TryGetBool(string _name, out bool _value) {
        bool result = parameterDictionary.TryGetValue(_name, out AnimatorParameter value);
        print("anim: " + parameterDictionary.Count);
        print(result);
        print(value.GetType());

        if (result) {
            _value = ((AnimatorBoolParameter)value).Value;
            // _value = value;
            return true;
        }

        _value = false;
        return false;
    }

    protected abstract void OnParametersSet();

    public void SetParameterDictionary(Dictionary<string, AnimatorParameter> _parameterDictionary) {
        // public void SetParameterDictionary(Dictionary<string, bool> _parameterDictionary) {
        parameterDictionary = _parameterDictionary;

        OnParametersSet();
    }
}