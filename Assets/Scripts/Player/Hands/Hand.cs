using System;
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
        torchActivateInputAction.action.performed += torch.OnTriggerPressed;

        playerSettings.OnPlayerTorchChanged.Register(ChangeHandTorch);

        ChangeHandTorch(playerSettings.playerTorchHand);
    }

    private void ChangeHandTorch(Side _side) {
        bool amITheTorchHand = side == _side;
        torchObj.SetActive(amITheTorchHand);
        if (amITheTorchHand) {
            torchActivateInputAction.action.Enable();
        } else {
            torchActivateInputAction.action.Disable();
        }
        handCollisionController.SetHandlerEnabled(handCollisionController.LadderHandler, amITheTorchHand); // toggle the collider for ladder on on the torchhand
        handCollisionController.SetHandlerEnabled(handCollisionController.PokeHandler, !amITheTorchHand); // toggle the collider for poke on the free hand
        handInteractionController.ToggleInteractions(!amITheTorchHand); // toggle the interactions on the free hand  
        handAnimatorController.UpdateGripHand(side, !amITheTorchHand);
    }

    void OnDestroy() {
        torchActivateInputAction.action.performed += torch.OnTriggerPressed;
        playerSettings.OnPlayerTorchChanged.Unregister(ChangeHandTorch);
    }

}
