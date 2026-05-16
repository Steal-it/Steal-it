using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour, ICollisionListener {

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
    private HingeJoint handleHingeJoint;
    [SerializeField]
    private Collider topTrigger;
    [SerializeField]
    private Collider bottomTrigger;
    [SerializeField]
    private Collider handleCollider;
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

    private float bottomPosition;
    private float topPosition;

    private void OnValidate() {
        startAtMiddle = leverActivationPoint == LeverActivationPoint.Both;
    }

    void Start() {
        bottomPosition = handleHingeJoint.limits.min;
        topPosition = handleHingeJoint.limits.max;

        if (!startAtMiddle) {
            SetSpringPoint(leverActivationPoint == LeverActivationPoint.Top ? bottomPosition : topPosition);
        }

        if (handleCollider.TryGetComponent<ColliderBridge>(out ColliderBridge triggerListener)) {
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

    public void OnCollisionEnter(Collision collision) {
        return;
    }

    public void OnCollisionExit(Collision collision) {
        return;
    }

    public void OnTriggerEnter(Collider other) {
        SetSpringPoint(System.Object.ReferenceEquals(topTrigger, other) ? topPosition : bottomPosition);

        if (eventsOnBoth) {
            bool isTopEventsEmpty = activationEventsOnBoth.onBottomActivated.GetPersistentEventCount() == 0;
            bool isBottomEventsEmpty = activationEventsOnBoth.onBottomActivated.GetPersistentEventCount() == 0;
            if (System.Object.ReferenceEquals(topTrigger, other) && !isTopEventsEmpty) {
                activationEventsOnBoth.onTopActivated?.Invoke();
            } else if (System.Object.ReferenceEquals(bottomTrigger, other) && !isBottomEventsEmpty) {
                activationEventsOnBoth.onBottomActivated?.Invoke();
            } else {
                onLeverActivation?.Invoke();
            }
        } else {
            onLeverActivation?.Invoke();
        }
    }

    public void OnTriggerExit(Collider other) {
        if (leverActivationPoint == LeverActivationPoint.Both) {
            SetSpringPoint(0);
        } else {
            SetSpringPoint(leverActivationPoint == LeverActivationPoint.Top ? bottomPosition : topPosition);
        }

        if (eventsOnBoth) {
            bool isTopEventsEmpty = deactivationEventsOnBoth.onTopDeactivated.GetPersistentEventCount() == 0;
            bool isBottomEventsEmpty = deactivationEventsOnBoth.onBottomDeactivated.GetPersistentEventCount() == 0;
            if (System.Object.ReferenceEquals(topTrigger, other) && !isTopEventsEmpty) {
                deactivationEventsOnBoth.onTopDeactivated?.Invoke();
            } else if (System.Object.ReferenceEquals(bottomTrigger, other) && !isBottomEventsEmpty) {
                deactivationEventsOnBoth.onBottomDeactivated?.Invoke();
            } else {
                onLeverDeactivation?.Invoke();
            }
        } else {
            onLeverDeactivation?.Invoke();
        }
    }

    private void SetSpringPoint(float springPoint) {
        JointSpring spring = handleHingeJoint.spring;
        spring.targetPosition = springPoint;
        handleHingeJoint.spring = spring;
    }
}
