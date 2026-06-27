using UnityEngine;

public class KeyLockManager : MonoBehaviour {
    [SerializeField]
    private Unlockable unlockable;
    [SerializeField]
    private Lock[] locks;

    void Start() {
        foreach (var l in locks) {
            l.OnUnlock += TryToUnlock;
        }
    }

    private void TryToUnlock() {
        foreach (var l in locks) {
            if (!l.IsUnlocked) return;
        }
        unlockable.Unlock();
    }
}
