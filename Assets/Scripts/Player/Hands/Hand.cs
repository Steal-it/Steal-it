using System;
using UnityEngine;

public class Hand : MonoBehaviour {
    [SerializeField]
    private PlayerSettingsSO playerSettings;
    [SerializeField]
    private Side side;
    private HandCollisionController handCollisionController;
    private HandInteractionController handInteractionController;

    void Awake() {
        TryGetComponent(out handCollisionController);
        TryGetComponent(out handInteractionController);
    }

    void Start() {
        handCollisionController.OnLadder += (_onLadder) => {
            handInteractionController.ToggleInteractions(_onLadder);
        };


        handCollisionController.OnCustomAction += handInteractionController.TorchInputAction.ChangeCurrentAction;

        playerSettings.OnPlayerTorchChanged.Register(ChangeHandTorch);
        ChangeHandTorch(playerSettings.playerTorchHand);
    }

    private void ChangeHandTorch(Side _side) {
        bool amITheTorchHand = side == _side;

        handInteractionController.ToggleInteractions(!amITheTorchHand); // toggle the interactions on the free hand  

        handCollisionController.SetHandlerEnabled(handCollisionController.LadderHandler, amITheTorchHand); // toggle the collider for ladder on on the torchhand
        handCollisionController.SetHandlerEnabled(handCollisionController.PokeHandler, !amITheTorchHand); // toggle the collider for poke on the free hand
        handCollisionController.SetHandlerEnabled(handCollisionController.CustomActionHandler, !amITheTorchHand); // toggle the collider for goggle on the free hand
        handCollisionController.RecalculateCollisions();
    }

    void OnDestroy() {
        playerSettings.OnPlayerTorchChanged.Unregister(ChangeHandTorch);
    }
}
