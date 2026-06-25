using UnityEngine;

public class AngularLever : Lever, ICollisionListener {
    [Space(10)]
    [SerializeField]
    private HingeJoint handleHingeJoint;

    protected override void Init() {
        bottomPosition = handleHingeJoint.limits.min;
        topPosition = handleHingeJoint.limits.max;
    }

    protected override void SetSpringPoint(float _springPoint) {
        JointSpring spring = handleHingeJoint.spring;
        spring.targetPosition = _springPoint;
        handleHingeJoint.spring = spring;
    }

}
