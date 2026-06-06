using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hand : CustomAction {
    [SerializeField]
    private PlayerSettingsSO playerSettings;
    [SerializeField]
    private Side side;
    [SerializeField]
    private GameObject torchObj;

    private HandCollisionController handCollisionController;
    private HandInteractionController handInteractionController;
    private HandAnimatorController handAnimatorController;
    private Torch torch;

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
            handAnimatorController.ToggleLadderAnimation(_onLadder);
        };

        handCollisionController.OnPoke += handAnimatorController.CalculatePoke;

        playerSettings.OnPlayerTorchChanged.Register(ChangeHandTorch);

        ChangeHandTorch(playerSettings.playerTorchHand);
    }

    private void ChangeHandTorch(Side _side) {
        bool amITheTorchHand = side == _side;
        torchObj.SetActive(amITheTorchHand);
        if (amITheTorchHand) {
            InputSetActive(true);
        } else {
            InputSetActive(false);
        }
        handCollisionController.SetHandlerEnabled(handCollisionController.LadderHandler, amITheTorchHand); // toggle the collider for ladder on on the torchhand
        handCollisionController.SetHandlerEnabled(handCollisionController.PokeHandler, !amITheTorchHand); // toggle the collider for poke on the free hand
        handInteractionController.ToggleInteractions(!amITheTorchHand); // toggle the interactions on the free hand  
        handAnimatorController.UpdateGripHand(side, !amITheTorchHand);
    }

    void OnDestroy() {
        playerSettings.OnPlayerTorchChanged.Unregister(ChangeHandTorch);
    }

    public override void OnInputFired(InputAction.CallbackContext ctx) {
        torch.OnTriggerPressed(ctx);
    }

    public override void OnInputStop(InputAction.CallbackContext ctx) {
        return;
    }
}
