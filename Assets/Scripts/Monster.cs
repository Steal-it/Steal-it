using System;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour {
    public event EventHandler OnAgentStopped;
    public event EventHandler OnAgentKilled;

    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField, Range(10, 30)]
    private float viewRadius = 20;
    [SerializeField, Range(0, 360)]
    private float viewAngle = 150;
    [SerializeField, Range(1, 3)]
    private float minChasingSpeed = 1.5f;
    [SerializeField, Range(3, 6)]
    private float maxChasingSpeed = 5;
    [SerializeField, Range(1, 3)]
    private float chasingAcceleration = 2;
    [SerializeField, Range(0, 4)]
    private float killDistance = 1.5f;

    private Transform avatarManagerTransform;
    private Transform player;

    void OnValidate() {
        if (minChasingSpeed > maxChasingSpeed) {
            minChasingSpeed = maxChasingSpeed;
        }
    }

    void Start() {
        avatarManagerTransform = NetworkReferenceManager.Instance.AvatarManager.transform;
    }

    void Update() {
        if (agent.isStopped) return;

        if (Vector3.Distance(agent.destination, transform.position) < agent.stoppingDistance) {
            agent.isStopped = true;

            OnAgentStopped?.Invoke(this, EventArgs.Empty);
        }

        if (player != null) {
            agent.destination = player.position;
            float newSpeed = agent.speed + Time.deltaTime / chasingAcceleration;
            agent.speed = newSpeed > maxChasingSpeed ? maxChasingSpeed : newSpeed;

            if (Vector3.Distance(transform.position, player.position) < killDistance) {
                agent.isStopped = true;
                agent.destination = transform.position;

                OnAgentKilled?.Invoke(this, EventArgs.Empty);
                print("KILLED");
            }

            return;
        }

        // Loop over all players (children of avatar manager)
        foreach (Transform avatar in avatarManagerTransform) {
            TorsoIdentifier torso = avatar.GetComponentInChildren<TorsoIdentifier>();
            if (IsInFOV(torso.transform.position)) {
                player = torso.transform;

                // Slow down before targeting the player not to overshoot
                agent.speed = minChasingSpeed;
                agent.autoBraking = false;
                return;
            }
        }
    }

    private bool IsInFOV(Vector3 _targetPosition) {
        Vector3 dirToTarget = _targetPosition - transform.position;

        // Check distance (outside the arc)
        if (dirToTarget.magnitude > viewRadius) {
            return false;
        }

        // Check angle (outside the cone)
        float angle = Vector3.Angle(transform.forward, dirToTarget);
        if (angle > viewAngle / 2f) {
            return false;
        }

        return true;
    }

    public void SetDestination(Vector3 _targetPosition) {
        agent.isStopped = false;
        agent.destination = _targetPosition;
    }

    void OnDrawGizmosSelected() {
        int numLines = 6;
        float angleOffset = viewAngle / numLines;
        for (int i = 0; i < numLines + 1; i++) {
            Vector3 line = DirFromAngle(-viewAngle / 2 + angleOffset * i);

            Gizmos.DrawLine(transform.position, transform.position + line * viewRadius);
        }

        Vector3 DirFromAngle(float _angleDegrees) {
            // Offset by the agent's current Y rotation so the cone rotates with it
            _angleDegrees += transform.eulerAngles.y;
            return new Vector3(Mathf.Sin(_angleDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(_angleDegrees * Mathf.Deg2Rad));
        }
    }
}
