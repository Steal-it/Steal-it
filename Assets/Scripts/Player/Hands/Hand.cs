using UnityEngine;
using UnityEngine.InputSystem;

public class Hand : MonoBehaviour {
    [SerializeField]
    private PlayerSettingsSO playerSettings;
    [SerializeField]
    private Side side;
    [SerializeField]
    private GameObject torchObj;
    [SerializeField]
    private InputActionReference torchActivateInputAction;

    private HandCollisionController handCollisionController;
    private HandInteractionController handInteractionController;
    private HandAnimatorController handAnimatorController;
    private Torch torch;
    private bool firstSpowned = true;

    void Awake() {
        TryGetComponent(out handCollisionController);
        TryGetComponent(out handInteractionController);
        TryGetComponent(out handAnimatorController);
        torchObj.TryGetComponent(out torch);
    }

    void Start() {
        handInteractionController.Init(side);

        handCollisionController.OnLadder += (_onLadder) => {
            handInteractionController.ToggleInteractions(_onLadder);
            handAnimatorController.ToggleFreeHandAnimation(_onLadder);
        };

        handCollisionController.OnPoke += handAnimatorController.CalculatePoke;
        torchActivateInputAction.action.performed += torch.OnTriggerPressed;

        playerSettings.OnPlayerTorchChanged.Register(ChangeHandTorch);

        ChangeHandTorch(playerSettings.playerTorchHand);
    }

    private void ChangeHandTorch(Side side) {
        bool amITheTorchHand = this.side == side;
        torchObj.SetActive(amITheTorchHand);
        if (amITheTorchHand) {
            if (!firstSpowned) {
                firstSpowned = false;
                handAnimatorController.ToggleFreeHandAnimation(false); // toggle freehand animation only on free hand
            }
            handCollisionController.OnLadder += torch.ToggleTorchInPocket;
            torchActivateInputAction.action.Enable();
        } else {
            handCollisionController.OnLadder -= torch.ToggleTorchInPocket;
            torchActivateInputAction.action.Disable();
        }
        handCollisionController.ToggleCollider(amITheTorchHand); // toggle the collider for ladder on on the torchhand
        handInteractionController.ToggleInteractions(!amITheTorchHand); // toggle the interactions on the free hand
        torch.ToggleTorchInPocket(!amITheTorchHand); // is in pocket if 

        handAnimatorController.UpdateFreeHandSide(side);

    }

    void OnDestroy() {
        torchActivateInputAction.action.performed += torch.OnTriggerPressed;
        playerSettings.OnPlayerTorchChanged.Unregister(ChangeHandTorch);
    }

}
