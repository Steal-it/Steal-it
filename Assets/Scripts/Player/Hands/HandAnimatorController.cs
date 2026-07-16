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

    private Transform parent;

    public void UpdateGripHand(Side side, bool _isThisHandFree) {
        FreeHandAnimator.ToggleTorchPosition(!_isThisHandFree);
        TorchAnimator.ToggleTorchVisible(!_isThisHandFree); // torch is visible only on torchhand
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


    public void OnGripReceived(Side side, object _sender, MessageHandler.OnAvatarComponentEnablerMessageReceivedEventArgs _args) {
        if (_args.ComponentType != AvatarComponentType.GripHand) {
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

        UpdateGripHand(side, _args.IsActive);
    }


    private void OnGripUpdate(InputVar<float> _grip) {
        if (!_grip.valid) {
            FreeHandAnimator.TargetGrip = 0;
            return;
        }
        FreeHandAnimator.TargetGrip = _grip.value;
    }
}
