public interface IMonsterStateVisitor {
    void Visit(WanderState _wanderState);
    void Visit(ChaseState _chaseState);
}
