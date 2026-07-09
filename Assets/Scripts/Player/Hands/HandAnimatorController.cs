using Ubiq;
using UnityEngine;

public class HandAnimatorController : MonoBehaviour {

    [SerializeField]
    private float pokeAnimationOffset;
    [SerializeField]
    private FreeHandAnimator FreeHandAnimator;
    [SerializeField]
    private TorchAnimator TorchAnimator;
    [SerializeField]
    private HeadAndHandsAvatar HeadAndHandsAvatar;

    public void ToggleHandStateAnimation(bool _freeHand) {
        FreeHandAnimator.ToggleTorchPosition(!_freeHand); // position only on torchhand
        TorchAnimator.ToggleTorchVisible(!_freeHand); // torch is visible only on torchhand
    }

    public void UpdateGripHand(Side side, bool _isThisHandFree) {
        FreeHandAnimator.ToggleTorchPosition(!_isThisHandFree);
        if (_isThisHandFree) {
            if (side == Side.Left) {
                HeadAndHandsAvatar.OnLeftGripUpdate.AddListener(OnGripUpdate);
            } else {
                HeadAndHandsAvatar.OnRightGripUpdate.AddListener(OnGripUpdate);
            }
        } else {
            if (side == Side.Left) {
                HeadAndHandsAvatar.OnLeftGripUpdate?.RemoveListener(OnGripUpdate);
            } else {
                HeadAndHandsAvatar.OnRightGripUpdate?.RemoveListener(OnGripUpdate);
            }
        }
    }

    public void CalculatePoke(float _distance, float _maxDistance) {
        if (_distance >= 0) {
            float pokeDistance = _distance - pokeAnimationOffset;
            float targetPoke = Mathf.InverseLerp(_maxDistance, 0, pokeDistance);
            FreeHandAnimator.TargetPoke = targetPoke;
        } else {
            FreeHandAnimator.TargetPoke = 0;
        }
    }


    private void OnGripUpdate(InputVar<float> _grip) {
        if (!_grip.valid) {
            FreeHandAnimator.TargetGrip = 0;
            return;
        }
        FreeHandAnimator.TargetGrip = _grip.value;
    }
}
