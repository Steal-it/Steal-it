using System;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IMonsterState, IMonsterStateVisitor {
    public event EventHandler OnPlayerKilled;

    public Transform Player => player;

    private MonsterStateManager monsterStateManager;
    private MonsterAI monster;
    private NavMeshAgent agent;
    private Transform player;
    private bool isChangingState;

    public void EnterState(MonsterStateManager _monsterStateManager) {
        monsterStateManager = _monsterStateManager;

        monster = monsterStateManager.MonsterAI;
        agent = monsterStateManager.Agent;

        isChangingState = false;
        monsterStateManager.WanderAndStunnedNavMeshSurface.enabled = false;
        monsterStateManager.ChaseNavMeshSurface.enabled = true;
        agent.speed = monsterStateManager.MinChasingSpeed;
        agent.autoBraking = false;
        monsterStateManager.MonsterAnimator.SetChase();

        IMonsterState wanderState = monsterStateManager.StateDictionary[MonsterStateManager.StateKey.Wander];
        wanderState.Accept(this);
    }

    public void UpdateState() {
        if (isChangingState) return;

        if (player == null) return;

        agent.destination = player.position;
        // Increase speed over time when chasing the player
        float newSpeed = agent.speed + Time.deltaTime / monsterStateManager.ChasingAcceleration;
        agent.speed = newSpeed > monsterStateManager.MaxChasingSpeed ? monsterStateManager.MaxChasingSpeed : newSpeed;

        if (Vector3.Distance(monster.transform.position, player.position) < monsterStateManager.KillDistance) {
            isChangingState = true;

            OnPlayerKilled?.Invoke(this, EventArgs.Empty);

            string playerUUID = player.parent.parent.gameObject.name;

            if (playerUUID != "Local Avatar") {
                playerUUID = playerUUID.Split('#')[1];
            } else {
                playerUUID = NetworkReferenceManager.Instance.RoomClient.Me.uuid;
            }

            SpectatorModeManager.Instance.ChangeSpectatorModeByPlayerUUID(playerUUID, true);

            monsterStateManager.ChangeState(MonsterStateManager.StateKey.Murder);
        }
    }

    public void ExitState() {
        agent.destination = monster.transform.position;
        monsterStateManager.ChaseNavMeshSurface.enabled = false;
    }

    public void Accept(IMonsterStateVisitor _stateVisitor) { }

    public void Visit(WanderState _wanderState) {
        player = _wanderState.Player;
    }
}