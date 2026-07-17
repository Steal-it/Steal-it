using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(NetworkObjectEnabler))]
public class TorchAnimator : LocalAvatar {
    [SerializeField]
    private Transform torchTransform;
    [SerializeField]
    private Transform torchAttachPoint;
    [SerializeField]
    private Transform pocketAttachPoint;
    [SerializeField]
    private Light torchLight;
    [SerializeField]
    private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField]
    private float duration = 1f;
    private bool active;

    private Tween moveTween;
    private NetworkObjectEnabler networkObjectEnabler;

    protected override void Awake() {
        base.Awake();
        TryGetComponent(out networkObjectEnabler);
    }

    void Start() {
        if (!torchTransform) {
            torchTransform = transform;
        }

        torchTransform.SetPositionAndRotation(torchAttachPoint.position, torchAttachPoint.rotation);

        if (!IsLocal) {
            networkObjectEnabler.OnMessageReceived += (_active) => {
                torchLight.enabled = _active;
                active = _active;
            };
        }
    }

    public void ToggleTorchVisible(bool _value) {
        moveTween?.Kill();

        InPocket(!_value);
        if (_value) {
            moveTween = torchTransform.DOLocalMove(torchAttachPoint.localPosition, duration).SetSpeedBased().SetEase(curve)
                        .OnStart(() => torchTransform.gameObject.SetActive(_value));
        } else {
            moveTween = torchTransform.DOLocalMove(pocketAttachPoint.localPosition, duration).SetSpeedBased().SetEase(curve)
                        .OnComplete(() => torchTransform.gameObject.SetActive(_value));
        }
    }

    public void SetupTorch(TorchLight _torchLight) {
        torchLight.range = _torchLight.MaxLightDistance;
        torchLight.spotAngle = _torchLight.LightAngle;
        torchLight.innerSpotAngle = _torchLight.LightAngle;
    }

    public void ToggleLightVisual(object sender, Torch.OnTorchTurnedEventArgs _eventArgs) {
        torchLight.enabled = _eventArgs.isTurnedOn;
        active = _eventArgs.isTurnedOn;
        networkObjectEnabler.SendEnableParameters(torchLight.enabled);
    }

    private void InPocket(bool _isInPocket) {
        if (_isInPocket) {
            torchLight.enabled = false;
        } else {
            torchLight.enabled = active;
        }
    }
}
