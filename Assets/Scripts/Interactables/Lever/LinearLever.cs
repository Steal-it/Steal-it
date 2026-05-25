using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class LinearLever : Lever, ICollisionListener {

    [Space(10)]
    [SerializeField]
    private ConfigurableJoint handleJoint;

    protected override void Init() {
        bottomPosition = handleJoint.linearLimit.limit;
        topPosition = -handleJoint.linearLimit.limit;
    }

    protected override void SetSpringPoint(float springPoint) {
        handleJoint.targetPosition = new Vector3(0, springPoint, 0);
    }
}
