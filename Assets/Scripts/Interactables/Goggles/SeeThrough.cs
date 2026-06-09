using DG.Tweening;
using UnityEngine;

public class SeeThrough : MonoBehaviour {
    [SerializeField]
    private float uiDistance;
    void Awake() {
        DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(10, 10);
    }

    public void EnableSeeThrough() {

    }
    public void DisableSeeThrough() {

    }
}
