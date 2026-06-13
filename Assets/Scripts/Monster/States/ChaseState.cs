using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IMonsterState, IMonsterStateVisitor {
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
        monsterStateManager.ChaseNavMeshSurface.enabled = true;
        agent.speed = monsterStateManager.MinChasingSpeed;
        agent.autoBraking = false;

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

            Debug.Log($"PLAYER KILLED");
            monsterStateManager.ChangeState(MonsterStateManager.StateKey.Stunned);
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