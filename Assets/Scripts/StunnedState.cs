using UnityEngine;
using UnityEngine.AI;

public class StunnedState : IMonsterState, IMonsterStateVisitor {
    private MonsterStateManager monsterStateManager;
    private Monster monster;
    private NavMeshAgent agent;
    private Transform player;
    private bool isChangingState;

    public void EnterState(MonsterStateManager _monsterStateManager) {
        monsterStateManager = _monsterStateManager;

        monster = monsterStateManager.Monster;
        agent = monsterStateManager.Agent;

        isChangingState = false;
        monsterStateManager.WanderAndStunnedNavMeshSurface.enabled = true;
        agent.speed = monsterStateManager.WanderAndStunnedSpeed;
        agent.autoBraking = true;

        IMonsterState chaseState = monsterStateManager.StateDictionary[MonsterStateManager.StateKey.Chase];
        chaseState.Accept(this);
    }

    public void UpdateState() {
        if (isChangingState) return;

        if (player == null) return;

        // TODO: turn back and go away
    }

    public void ExitState() { }

    public void Accept(IMonsterStateVisitor _stateVisitor) { }

    public void Visit(WanderState _wanderState) { }

    public void Visit(ChaseState _chaseState) {
        player = _chaseState.Player;
    }
}