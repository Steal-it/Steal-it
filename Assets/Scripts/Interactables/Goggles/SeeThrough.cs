using DG.Tweening;
using UnityEngine;

public class SeeThrough : MonoBehaviour {
    [SerializeField]
    private Transform seeThoughPanel;
    [SerializeField]
    private Ease activationEase;
    [SerializeField]
    private Ease disableEase;
    [SerializeField]
    private float duration;
    void Awake() {
        DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(10, 10);
    }

    public void EnableSeeThrough() {
        seeThoughPanel.DOScaleX(0.05f, duration).SetEase(activationEase);
    }
    public void DisableSeeThrough() {
        seeThoughPanel.DOScaleX(0, duration).SetEase(disableEase);
    }
}
