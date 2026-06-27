using UnityEngine;
using UnityEngine.AI;

public abstract class StunnedState : IMonsterState {
    protected MonsterStateManager MonsterStateManager { get; private set; }
    private MonsterAI monster;
    private NavMeshAgent agent;
    private bool isAnimationFinished;
    private bool isChangingState;

    private void PlayAnimation() {
        MonsterStateManager.StartCoroutine(FakeAnim());

        System.Collections.IEnumerator FakeAnim() {
            SetAnimation();

            yield return new WaitForSeconds(3);

            // Look for a random destination no earlier than the minimum StunnedMinDistanceDestination
            Vector3 randomDestination;
            bool isFarEnough;
            do {
                randomDestination = MonsterStateManager.MonsterRandomDestinationManager.GenerateRandomDestination();

                isFarEnough = Vector3.Distance(randomDestination, monster.transform.position) > MonsterStateManager.StunnedMinDistanceDestination;
            } while (!isFarEnough);
            agent.destination = randomDestination;

            isAnimationFinished = true;
        }
    }

    protected abstract void SetAnimation();

    public void EnterState(MonsterStateManager _monsterStateManager) {
        MonsterStateManager = _monsterStateManager;

        monster = MonsterStateManager.Monster;
        agent = MonsterStateManager.Agent;

        isChangingState = false;
        isAnimationFinished = false;
        MonsterStateManager.WanderAndStunnedNavMeshSurface.enabled = true;
        MonsterStateManager.ChaseNavMeshSurface.enabled = false;
        agent.speed = MonsterStateManager.WanderAndStunnedSpeed;
        agent.autoBraking = true;
        PlayAnimation();
    }

    public void UpdateState() {
        if (isChangingState) return;

        if (!isAnimationFinished) return;

        if (Vector3.Distance(agent.destination, monster.transform.position) < agent.stoppingDistance) {
            isChangingState = true;

            MonsterStateManager.ChangeState(MonsterStateManager.StateKey.Wander);
        }
    }

    public void ExitState() {
        agent.destination = monster.transform.position;
    }

    public void Accept(IMonsterStateVisitor _stateVisitor) { }
}