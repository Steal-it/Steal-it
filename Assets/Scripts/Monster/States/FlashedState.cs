public class FlashedState : StunnedState {
    protected override void SetAnimation() {
        MonsterStateManager.MonsterAnimator.SetFlashed();
    }
}
