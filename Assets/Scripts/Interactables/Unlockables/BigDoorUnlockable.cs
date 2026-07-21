using DG.Tweening;
using UnityEngine;

public class BigDoorUnlockable : Unlockable {
    [SerializeField]
    private Transform leftDoor;
    [SerializeField]
    private Transform rightDoor;
    [SerializeField, Range(2, 4)]
    private float duration = 3;

    public override void Unlock() {
        leftDoor.DOLocalRotate(new Vector3(0, -90, 0), duration).SetEase(Ease.InOutQuad);
        rightDoor.DOLocalRotate(new Vector3(0, 90, 0), duration).SetEase(Ease.InOutQuad);
    }
}
