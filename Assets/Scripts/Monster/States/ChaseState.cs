using System;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IMonsterState, IMonsterStateVisitor {
    public Transform Player => player;

    private MonsterStateManager monsterStateManager;
    private NavMeshAgent monsterAgent;
    private Transform player;
    private bool isChangingState;

    public void EnterState(MonsterStateManager _monsterStateManager) {
        monsterStateManager = _monsterStateManager;

        monsterAgent = monsterStateManager.MonsterAgent;

        isChangingState = false;
        monsterStateManager.WanderAndStunnedNavMeshSurface.enabled = false;
        monsterStateManager.ChaseNavMeshSurface.enabled = true;
        monsterAgent.speed = monsterStateManager.MinChasingSpeed;
        monsterAgent.autoBraking = false;
        monsterStateManager.MonsterAnimator.SetChase();
        monsterStateManager.MonsterSFXManager.SetChase(true);

        IMonsterState wanderState = monsterStateManager.StateDictionary[MonsterStateManager.StateKey.Wander];
        wanderState.Accept(this);
    }

    public void UpdateState() {
        if (isChangingState) return;

        if (player == null) return;

        // TODO: player might not be reachable because out of navmesh surface
        monsterAgent.destination = player.position;
        // Increase speed over time when chasing the player
        float newSpeed = monsterAgent.speed + Time.deltaTime / monsterStateManager.ChasingAcceleration;
        monsterAgent.speed = newSpeed > monsterStateManager.MaxChasingSpeed ? monsterStateManager.MaxChasingSpeed : newSpeed;

        if (Vector3.Distance(monsterAgent.transform.position, player.position) < monsterStateManager.KillDistance) {
            isChangingState = true;

            string playerUUID = player.parent.parent.gameObject.name;

            if (playerUUID != "Local Avatar") {
                playerUUID = playerUUID.Split('#')[1];
            } else {
                playerUUID = NetworkReferenceManager.Instance.RoomClient.Me.uuid;
            }

            NetworkReferenceManager.Instance.SpectatorModeManager.ChangeSpectatorModeByPlayerUUID(playerUUID, true);

            monsterStateManager.ChangeState(MonsterStateManager.StateKey.Murder);
        }
    }

    public void ExitState() {
        monsterAgent.destination = monsterAgent.transform.position;
        monsterStateManager.ChaseNavMeshSurface.enabled = false;
        monsterStateManager.MonsterSFXManager.SetWander(false);
        monsterStateManager.MonsterSFXManager.SetChase(false);
    }

    public void Accept(IMonsterStateVisitor _stateVisitor) { }

    public void Visit(WanderState _wanderState) {
        player = _wanderState.Player;
    }
}