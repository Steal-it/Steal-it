using DG.Tweening;
using UnityEngine;

public class SeeThrough : MonoBehaviour {
    [SerializeField]
    private Transform seeThoughPanel;
    [SerializeField]
    private AnimationCurve orizzontalCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField]
    private AnimationCurve verticalCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField]
    private float duration;

    private Sequence seeThrough;

    public void Start() {
        seeThrough = DOTween.Sequence();
        seeThrough.Append(seeThoughPanel.DOScaleX(0.01f, duration).SetEase(orizzontalCurve));
        seeThrough.Join(seeThoughPanel.DOScaleZ(0.01f, duration).SetEase(verticalCurve));
        seeThrough.Pause();
        seeThrough.SetAutoKill(false);
    }

    public void EnableSeeThrough() {
        seeThrough.PlayForward();
    }
    public void DisableSeeThrough() {
        seeThrough.PlayBackwards();
    }
}
