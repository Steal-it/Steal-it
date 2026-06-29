using UnityEngine;
using UnityEngine.AI;

public class WanderState : IMonsterState {
    public Transform Player => player;

    private MonsterStateManager monsterStateManager;
    private MonsterAI monster;
    private NavMeshAgent agent;
    private Transform player;
    private bool isChangingState;

    public void EnterState(MonsterStateManager _monsterStateManager) {
        monsterStateManager = _monsterStateManager;

        monster = monsterStateManager.Monster;
        agent = monsterStateManager.Agent;

        isChangingState = false;
        monsterStateManager.WanderAndStunnedNavMeshSurface.enabled = true;
        monsterStateManager.ChaseNavMeshSurface.enabled = false;
        agent.speed = monsterStateManager.WanderAndStunnedSpeed;
        agent.autoBraking = true;
        agent.destination = monster.transform.position;
        monsterStateManager.MonsterAnimator.SetWander();
    }

    public void UpdateState() {
        if (isChangingState) return;

        if (Vector3.Distance(agent.destination, monster.transform.position) < agent.stoppingDistance) {
            Vector3 randomDestination = monsterStateManager.MonsterRandomDestinationManager.GenerateRandomDestination();
            agent.destination = randomDestination;
        }

        // Loop over all players (children of avatar manager)
        foreach (Transform avatar in NetworkReferenceManager.Instance.AvatarManager.transform) {
            // Skip the checks if the avatar is not visible (player in spectator mode)
            if (!avatar.gameObject.activeInHierarchy) continue;

            TorsoIdentifier torso = avatar.GetComponentInChildren<TorsoIdentifier>();
            if (IsInFOV(torso.transform.position)) {
                isChangingState = true;

                player = torso.transform;

                monsterStateManager.ChangeState(MonsterStateManager.StateKey.Chase);

                break;
            }
        }
    }


    private bool IsInFOV(Vector3 _targetPosition) {
        Vector3 dirToTarget = _targetPosition - monster.transform.position;

        // Check distance (outside the arc)
        if (dirToTarget.magnitude > monsterStateManager.ViewRadius) {
            return false;
        }

        // Check angle (outside the cone)
        float angle = Vector3.Angle(monster.transform.forward, dirToTarget);
        if (angle > monsterStateManager.ViewAngle / 2f) {
            return false;
        }

        // Check avatar (not hidden by a wall)
        // Offsetting the origin of the ray a bit behind the monster ensures that, even when the monster pops out from a wall,
        // it fully crosses the wall before targeting the player, so the NavMesh Surface change looks smooth
        Vector3 origin = monster.transform.position + dirToTarget.normalized * -1.5f + Vector3.up;
        float playerDistance = Vector3.Distance(origin, _targetPosition + Vector3.up);
        if (Physics.Raycast(origin, dirToTarget, playerDistance, monsterStateManager.WallLayer)) {
            return false;
        }

        return true;
    }

    public void ExitState() {
        monsterStateManager.WanderAndStunnedNavMeshSurface.enabled = false;
    }

    public void Accept(IMonsterStateVisitor _stateVisitor) {
        _stateVisitor.Visit(this);
    }
}
