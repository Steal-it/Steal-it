using UnityEngine;
using DG.Tweening;
using System;

public class TorchAnimator : LocalAvatar {
    public event Action<bool> OnTorchPocket;

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
    private Transform parent;
    private Tween moveTween;

    protected override void Awake() {
        base.Awake();
    }

    void Start() {
        if (!torchTransform) {
            torchTransform = transform;
        }

        torchTransform.SetPositionAndRotation(torchAttachPoint.position, torchAttachPoint.rotation);
    }

    public void OnAvatarComponentEnablerMessageReceived(object _sender, MessageHandler.OnAvatarComponentEnablerMessageReceivedEventArgs _args) {
        if (_args.ComponentType != AvatarComponentType.TorchLight) {
            return;
        }

        if (parent == null) {
            parent = transform;
        }

        string playerUUID = parent.name;

        while (!playerUUID.Contains("Remote Avatar") && !playerUUID.Contains("Local Avatar")) {
            parent = parent.parent;
            playerUUID = parent.name;
        }

        if (playerUUID != "Local Avatar") {
            playerUUID = playerUUID.Split('#')[1];
            if (playerUUID != _args.PlayerUUID) {
                return;
            }
        }

        if (_args.ComponentType == AvatarComponentType.TorchLight) {
            torchLight.enabled = _args.IsActive;
            active = _args.IsActive;
        }
    }

    public void ToggleTorchVisible(bool _value) {
        moveTween?.Kill();

        InPocket(!_value);
        if (_value) {
            moveTween = torchTransform.DOLocalMove(torchAttachPoint.localPosition, duration).SetSpeedBased().SetEase(curve)
                        .OnStart(() => torchTransform.gameObject.SetActive(_value)).OnComplete(() => OnTorchPocket?.Invoke(!_value));
        } else {
            moveTween = torchTransform.DOLocalMove(pocketAttachPoint.localPosition, duration).SetSpeedBased().SetEase(curve)
                        .OnStart(() => OnTorchPocket?.Invoke(!_value)).OnComplete(() => torchTransform.gameObject.SetActive(_value));
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
        NetworkReferenceManager.Instance.MessageHandler.SendAvatarComponentEnablerMessage(AvatarComponentType.TorchLight, torchLight.enabled);
    }

    private void InPocket(bool _isInPocket) {
        if (_isInPocket) {
            torchLight.enabled = false;
        } else {
            torchLight.enabled = active;
        }
    }

    void OnDisable() {
        NetworkReferenceManager.Instance.MessageHandler.OnAvatarComponentEnablerMessageReceived -= OnAvatarComponentEnablerMessageReceived;
    }

}
