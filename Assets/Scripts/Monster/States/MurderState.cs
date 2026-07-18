public class MurderState : StunnedState {
    protected override void SetAnimation() {
        MonsterStateManager.MonsterAnimator.SetMurder();
    }

    protected override void SetSFX(bool _isActive) {
        MonsterStateManager.MonsterSFXManager.SetMurder(_isActive);
    }
}
