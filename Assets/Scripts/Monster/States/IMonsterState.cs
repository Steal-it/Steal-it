public interface IMonsterState {
    void EnterState(MonsterStateManager _monsterStateManager);
    void UpdateState();
    void ExitState();
    void Accept(IMonsterStateVisitor _stateVisitor);
}
