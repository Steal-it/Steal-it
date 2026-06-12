using UnityEngine;
using UnityEngine.AI;

public class StunnedState : IMonsterState {
    private MonsterStateManager monsterStateManager;
    private Monster monster;
    private NavMeshAgent agent;
    private bool isAnimationFinished;
    private bool isChangingState;

    private void PlayAnimation() {
        monsterStateManager.StartCoroutine(FakeAnim());

        System.Collections.IEnumerator FakeAnim() {
            monsterStateManager.MonsterAnimator.SetIsStunned(true);

            yield return new WaitForSeconds(4);

            // Look for a random destination no earlier than the minimum StunnedMinDistanceDestination
            Vector3 randomDestination;
            bool isFarEnough;
            do {
                randomDestination = monsterStateManager.MonsterRandomDestinationManager.GenerateRandomDestination();

                isFarEnough = Vector3.Distance(randomDestination, monster.transform.position) > monsterStateManager.StunnedMinDistanceDestination;
            } while (!isFarEnough);
            agent.destination = randomDestination;

            isAnimationFinished = true;
        }
    }

    public void EnterState(MonsterStateManager _monsterStateManager) {
        monsterStateManager = _monsterStateManager;

        monster = monsterStateManager.Monster;
        agent = monsterStateManager.Agent;

        isChangingState = false;
        isAnimationFinished = false;
        monsterStateManager.WanderAndStunnedNavMeshSurface.enabled = true;
        agent.speed = monsterStateManager.WanderAndStunnedSpeed;
        agent.autoBraking = true;
        PlayAnimation();
    }

    public void UpdateState() {
        if (isChangingState) return;

        if (!isAnimationFinished) return;

        if (Vector3.Distance(agent.destination, monster.transform.position) < agent.stoppingDistance) {
            isChangingState = true;

            monsterStateManager.ChangeState(MonsterStateManager.StateKey.Wander);
        }
    }

    public void ExitState() {
        agent.destination = monster.transform.position;
        monsterStateManager.MonsterAnimator.SetIsStunned(false);
    }

    public void Accept(IMonsterStateVisitor _stateVisitor) { }
}