using DG.Tweening;
using UnityEngine;

public class SeeThrough : MonoBehaviour {
    [SerializeField]
    private Transform seeThoughPanel;
    [SerializeField]
    private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField]
    private float duration;

    public void EnableSeeThrough() {
        seeThoughPanel.DOScaleX(0.05f, duration).SetEase(curve);
    }
    public void DisableSeeThrough() {
        seeThoughPanel.DOScaleX(0, duration).SetEase(curve);
    }
}
