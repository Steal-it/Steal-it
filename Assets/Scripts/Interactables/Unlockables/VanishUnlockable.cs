public class VanishUnlockable : Unlockable {
    public override void Unlock() {
        gameObject.SetActive(false);
    }
}
