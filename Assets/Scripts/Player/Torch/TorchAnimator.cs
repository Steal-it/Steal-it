using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TorchAnimator : MonoBehaviour {

    [SerializeField]
    private Transform torchAttachPoint;
    [SerializeField]
    private Transform pocketAttachPoint;
    [SerializeField]
    private TorchLight torchLight;
    [SerializeField]
    private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField]
    private float duration = 1f;

    private Tween moveTween;


    void Start() {
        transform.SetPositionAndRotation(torchAttachPoint.position, torchAttachPoint.rotation);
    }

    public void ToggleTorchVisible(bool _value) {
        moveTween?.Kill();

        torchLight.InPocket(!_value);
        if (_value) {
            moveTween = transform.DOLocalMove(torchAttachPoint.localPosition, duration).SetSpeedBased().SetEase(curve)
                        .OnStart(() => gameObject.SetActive(_value));
        } else {
            moveTween = transform.DOLocalMove(pocketAttachPoint.localPosition, duration).SetSpeedBased().SetEase(curve)
                        .OnComplete(() => gameObject.SetActive(_value));
        }
    }

}
