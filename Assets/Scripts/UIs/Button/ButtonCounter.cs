using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ButtonCounter : MonoBehaviour {

    [SerializeField]
    private short pressRequirement;
    [Space(10f), SerializeField]
    private UnityEvent<short> OnButtonPress;
    [SerializeField]
    private UnityEvent OnPressRequirementSatisfied;

    private short pressCount = 0;

    public void IncrementCounter() {
        OnButtonPress?.Invoke(pressCount++);
        if (pressCount >= pressRequirement) {
            OnPressRequirementSatisfied?.Invoke();
        }
    }
}
