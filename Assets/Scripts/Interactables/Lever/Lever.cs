using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Lever : MonoBehaviour, ICollisionListener {

    enum LeverActivationPoint {
        Top,
        Bottom,
        Both
    }

    [Serializable]
    class BothActivationEvents {
        public UnityEvent onTopActivated;
        public UnityEvent onBottomActivated;
    }

    [Serializable]
    class BothDeactivationEvents {
        public UnityEvent onTopDeactivated;
        public UnityEvent onBottomDeactivated;
    }

    [SerializeField]
    private LeverActivationPoint leverActivationPoint;
    [Space(10)]
    [SerializeField]
    private Collider handleCollider;
    [SerializeField]
    private Collider topTrigger;
    [SerializeField]
    private Collider bottomTrigger;
    [SerializeField]
    private bool startAtMiddle;
    [Space(10)]
    [SerializeField]
    private bool eventsOnBoth;
    [SerializeField]
    private BothActivationEvents activationEventsOnBoth;
    [SerializeField]
    private BothDeactivationEvents deactivationEventsOnBoth;
    [Space(5)]
    [Header("Main Lever Events")]
    [Space(10)]
    [SerializeField]
    [Tooltip("This will be the fallback if one of the activation events is empty")]
    private UnityEvent onLeverActivation;
    [SerializeField]
    [Tooltip("This will be the fallback if one of the deactivation events is empty")]
    private UnityEvent onLeverDeactivation;

    protected float bottomPosition;
    protected float topPosition;

    private void OnValidate() {
        eventsOnBoth = leverActivationPoint == LeverActivationPoint.Both;
    }

    void Start() {
        Init();

        if (!startAtMiddle) {
            SetSpringPoint(leverActivationPoint == LeverActivationPoint.Top ? bottomPosition : topPosition);
        }

        if (handleCollider.TryGetComponent<ColliderBridge>(out var triggerListener)) {
            triggerListener.Initialize(this);
        }

        switch (leverActivationPoint) {
            case LeverActivationPoint.Top:
                bottomTrigger.enabled = false;
                break;
            case LeverActivationPoint.Bottom:
                topTrigger.enabled = false;
                break;
            case LeverActivationPoint.Both:
                break;
        }
    }

    public void OnCollisionEnterRec(Collision _collision) {
        return;
    }

    public void OnCollisionExitRec(Collision _collision) {
        return;
    }

    public void OnTriggerEnterRec(Collider _other) {
        if (!IsCollidingWithLevelTrigger(_other)) return;

        SetSpringPoint(ReferenceEquals(topTrigger, _other) ? topPosition : bottomPosition);

        if (eventsOnBoth) {
            bool isTopEventsEmpty = activationEventsOnBoth.onBottomActivated.GetPersistentEventCount() == 0;
            bool isBottomEventsEmpty = activationEventsOnBoth.onBottomActivated.GetPersistentEventCount() == 0;
            if (ReferenceEquals(topTrigger, _other) && !isTopEventsEmpty) {
                activationEventsOnBoth.onTopActivated?.Invoke();
            } else if (ReferenceEquals(bottomTrigger, _other) && !isBottomEventsEmpty) {
                activationEventsOnBoth.onBottomActivated?.Invoke();
            } else {
                onLeverActivation?.Invoke();
            }
        } else {
            onLeverActivation?.Invoke();
        }
    }

    public void OnTriggerExitRec(Collider _other) {
        if (!IsCollidingWithLevelTrigger(_other)) return;

        if (leverActivationPoint == LeverActivationPoint.Both) {
            SetSpringPoint(0);
        } else {
            SetSpringPoint(leverActivationPoint == LeverActivationPoint.Top ? bottomPosition : topPosition);
        }

        if (eventsOnBoth) {
            bool isTopEventsEmpty = deactivationEventsOnBoth.onTopDeactivated.GetPersistentEventCount() == 0;
            bool isBottomEventsEmpty = deactivationEventsOnBoth.onBottomDeactivated.GetPersistentEventCount() == 0;
            if (ReferenceEquals(topTrigger, _other) && !isTopEventsEmpty) {
                deactivationEventsOnBoth.onTopDeactivated?.Invoke();
            } else if (ReferenceEquals(bottomTrigger, _other) && !isBottomEventsEmpty) {
                deactivationEventsOnBoth.onBottomDeactivated?.Invoke();
            } else {
                onLeverDeactivation?.Invoke();
            }
        } else {
            onLeverDeactivation?.Invoke();
        }
    }

    private bool IsCollidingWithLevelTrigger(Collider _other) {
        return ReferenceEquals(topTrigger, _other) || ReferenceEquals(bottomTrigger, _other);
    }

    protected abstract void Init();

    protected abstract void SetSpringPoint(float _springPoint);

}
