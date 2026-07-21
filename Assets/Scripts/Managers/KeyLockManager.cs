using System.Collections.Generic;
using UnityEngine;

public class KeyLockManager : MonoBehaviour {
    public Transform LockList => lockList;

    [SerializeField]
    private Unlockable unlockable;
    [SerializeField]
    private Transform lockList;

    private List<Lock> locks = new List<Lock>();

    void Start() {
        foreach (Transform l in lockList) {
            locks.Add(l.GetComponent<Lock>());
        }

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
