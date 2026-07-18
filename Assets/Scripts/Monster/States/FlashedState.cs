public class FlashedState : StunnedState {
    protected override void SetAnimation() {
        MonsterStateManager.MonsterAnimator.SetFlashed();
    }

    protected override void SetSFX(bool _isActive) {
        MonsterStateManager.MonsterSFXManager.SetFlashed(_isActive);
    }
}
