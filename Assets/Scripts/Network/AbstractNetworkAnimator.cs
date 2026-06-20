using Ubiq.Dictionaries;
using UnityEngine;

public abstract class AbstractNetworkAnimator : MonoBehaviour {
    private SerializableDictionary parameterDictionary;
    // private Dictionary<string, bool> parameterDictionary;

    // protected bool TryGetBool(string _name, out AnimationMessage.AnimatorBoolParameter _value) {
    protected bool TryGetBool(string _name, out bool _value) {
        foreach (var e in parameterDictionary) {
            if (e.Key == _name) {
                _value = bool.Parse(e.Value);
                return true;
            }
        }

        _value = false;
        return false;
        // print("anim: " + parameterDictionary.Count);
        // print(result);
        // print(value.GetType());

        // if (result) {
        //     var c = (value as AnimatorBoolParameter);
        //     print(c);
        //     _value = ((AnimatorBoolParameter)value).Value;
        //     // _value = value;
        //     return true;
        // }

        // _value = false;
        // return false;
    }

    protected abstract void OnParametersSet();

    public void SetParameterDictionary(SerializableDictionary _parameterDictionary) {
        // public void SetParameterDictionary(Dictionary<string, bool> _parameterDictionary) {
        parameterDictionary = _parameterDictionary;

        OnParametersSet();
    }
}