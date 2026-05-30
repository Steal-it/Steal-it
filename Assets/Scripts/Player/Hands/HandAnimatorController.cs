using Ubiq;
using UnityEngine;

public class HandAnimatorController : MonoBehaviour {
    [SerializeField]
    private FreeHandAnimator freeHandAnimator;
    [SerializeField]
    private TorchAnimator torchAnimator;

    [SerializeField]
    private float pokeAnimationOffset;

    private HeadAndHandsAvatar headAndHandsAvatar;

    void Awake() {
        headAndHandsAvatar = GetComponentInParent<HeadAndHandsAvatar>();
    }

    public void ToggleHandAnimation(bool _freeHand) {
        freeHandAnimator.TorchPosition(!_freeHand); // position only on torchhand
        torchAnimator.TorchVisible(!_freeHand); // torch is visible only on torchhand
    }

    public void UpdateGripHand(Side side, bool _isThisHandFree) {
        freeHandAnimator.TorchPosition(!_isThisHandFree);
        if (_isThisHandFree) {
            if (side == Side.Left) {
                headAndHandsAvatar.OnLeftGripUpdate.AddListener(OnGripUpdate);
            } else {
                headAndHandsAvatar.OnRightGripUpdate.AddListener(OnGripUpdate);
            }
        } else {
            if (side == Side.Left) {
                headAndHandsAvatar.OnLeftGripUpdate?.RemoveListener(OnGripUpdate);
            } else {
                headAndHandsAvatar.OnRightGripUpdate?.RemoveListener(OnGripUpdate);
            }
        }
    }

    public void CalculatePoke(float _distance, float _maxDistance) {
        if (_distance >= 0) {
            float pokeDistance = _distance - pokeAnimationOffset;
            float targetPoke = Mathf.InverseLerp(_maxDistance, 0, pokeDistance);
            freeHandAnimator.TargetPoke = targetPoke;
        } else {
            freeHandAnimator.TargetPoke = 0;
        }
    }

    private void OnGripUpdate(InputVar<float> _grip) {
        if (!_grip.valid) {
            freeHandAnimator.TargetGrip = 0;
            return;
        }
        freeHandAnimator.TargetGrip = _grip.value;
    }
}
