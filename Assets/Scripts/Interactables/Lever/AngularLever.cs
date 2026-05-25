using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class AngularLever : Lever, ICollisionListener {
    [Space(10)]
    [SerializeField]
    private HingeJoint handleHingeJoint;

    protected override void Init() {
        bottomPosition = handleHingeJoint.limits.min;
        topPosition = handleHingeJoint.limits.max;
    }

    protected override void SetSpringPoint(float springPoint) {
        JointSpring spring = handleHingeJoint.spring;
        spring.targetPosition = springPoint;
        handleHingeJoint.spring = spring;
    }

}
