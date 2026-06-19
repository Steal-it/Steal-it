using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractNetworkAnimator : MonoBehaviour {
    private Dictionary<string, object> parameterDictionary;

    protected bool TryGetBool(string _name, out bool _value) {
        bool result = parameterDictionary.TryGetValue(_name, out object value);
        _value = (bool)value;
        return result;
    }

    protected abstract void OnParametersSet();

    public void SetParameterDictionary(Dictionary<string, object> _parameterDictionary) {
        parameterDictionary = _parameterDictionary;

        OnParametersSet();
    }
}