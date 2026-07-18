using UnityEngine;
using UnityEngine.AI;

public abstract class StunnedState : IMonsterState {
    protected MonsterStateManager MonsterStateManager { get; private set; }
    private NavMeshAgent monsterAgent;
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
            int remainingAttempts = 50;
            do {
                randomDestination = MonsterStateManager.MonsterRandomDestinationManager.GenerateRandomDestination();

                isFarEnough = Vector3.Distance(randomDestination, monsterAgent.transform.position) > MonsterStateManager.StunnedMinDistanceDestination;

                remainingAttempts--;
                if (remainingAttempts < 0) {
                    Debug.LogWarning("No point too far from player found!");
                    break;
                }
            } while (!isFarEnough);
            monsterAgent.destination = randomDestination;

            isAnimationFinished = true;
        }
    }

    protected abstract void SetAnimation();

    protected abstract void SetSFX(bool _isActive);

    public void EnterState(MonsterStateManager _monsterStateManager) {
        MonsterStateManager = _monsterStateManager;

        monsterAgent = MonsterStateManager.MonsterAgent;

        isChangingState = false;
        isAnimationFinished = false;
        MonsterStateManager.WanderAndStunnedNavMeshSurface.enabled = true;
        MonsterStateManager.ChaseNavMeshSurface.enabled = false;
        monsterAgent.speed = MonsterStateManager.WanderAndStunnedSpeed;
        monsterAgent.autoBraking = true;
        PlayAnimation();
        SetSFX(true);
    }

    public void UpdateState() {
        if (isChangingState) return;

        if (!isAnimationFinished) return;

        if (Vector3.Distance(monsterAgent.destination, monsterAgent.transform.position) < monsterAgent.stoppingDistance) {
            isChangingState = true;

            MonsterStateManager.ChangeState(MonsterStateManager.StateKey.Wander);
        }
    }

    public void ExitState() {
        monsterAgent.destination = monsterAgent.transform.position;
        SetSFX(false);
    }

    public void Accept(IMonsterStateVisitor _stateVisitor) { }
}