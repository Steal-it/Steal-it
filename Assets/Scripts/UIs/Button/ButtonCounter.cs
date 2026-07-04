using Ubiq.Messaging;
using UnityEngine;
using UnityEngine.Events;

public class ButtonCounter : MonoBehaviour {

    [SerializeField]
    private short pressRequirement;
    [Space(10f), SerializeField]
    private UnityEvent<short> OnButtonPress;
    [SerializeField]
    private UnityEvent OnPressRequirementSatisfied;
    private NetworkContext context;

    private short pressCount = 0;

    void Awake() {
        context = NetworkScene.Register(this);
    }

    public void IncrementCounter() {
        OnButtonPress?.Invoke(pressCount += 1);
        if (pressCount >= pressRequirement) {
            OnPressRequirementSatisfied?.Invoke();
        }

        context.SendJson(new IncrementCounterMessage(pressCount));
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        IncrementCounterMessage message = _message.FromJson<IncrementCounterMessage>();

        pressCount = message.newCounterValue;

        OnButtonPress?.Invoke(pressCount);
    }
}
