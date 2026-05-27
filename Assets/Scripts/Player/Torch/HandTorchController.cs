// using System;
// using Unity.XR.CoreUtils;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.XR.Interaction.Toolkit.Interactors;

// public class HandTorchController : MonoBehaviour {
//     [SerializeField]
//     private Side side;
//     [SerializeField]
//     private PlayerSettingsSO playerSettings;
//     [SerializeField]
//     private GameObject torchPrefab;
//     [SerializeField]
//     private InputActionReference controllerActivateInputAction;
//     [SerializeField]
//     private LayerMask ladderLayer;

//     private HandAnimator handAnimator;
//     private Torch torch;
//     private Collider detectorCollider;
//     private NearFarInteractor nearFarInteractor;

//     void OnEnable() {
//         playerSettings.OnPlayerTorchChanged.Register(ChangeHandTorch);
//     }

//     void Awake() {
//         TryGetComponent(out handAnimator);
//         TryGetComponent(out detectorCollider);
//         torchPrefab.TryGetComponent(out torch);
//     }

//     // private void Start() {
//     //     XROrigin origin = FindFirstObjectByType<XROrigin>();
//     //     if (side == Side.Left) {
//     //         nearFarInteractor = origin.transform.Find("Camera Offset/Left Controller").GetComponentInChildren<NearFarInteractor>();
//     //     } else {
//     //         nearFarInteractor = origin.transform.Find("Camera Offset/Right Controller").GetComponentInChildren<NearFarInteractor>();
//     //     }
//     //     // hand animator init
//     //     ChangeHandTorch(playerSettings.playerTorchHand);
//     // }

//     // void OnTriggerEnter(Collider _other) {
//     //     // Detect if the other object is a rung of a ladder
//     //     if (((1 << _other.gameObject.layer) & ladderLayer) != 0) {
//     //         EnableInteractions();
//     //         EnableHandTorch();
//     //     }
//     // }

//     // void OnTriggerExit(Collider _other) {
//     //     // Detect if the other object is a rung of a ladder
//     //     if (((1 << _other.gameObject.layer) & ladderLayer) != 0) {
//     //         DisableInteractions();
//     //         EnableFreeHand();
//     //     }
//     // }

//     private void EnableTorchHand() {
//         // DisableInteractions();

//         controllerActivateInputAction.action.Enable();
//         controllerActivateInputAction.action.performed += torch.OnTriggerPressed;

//         detectorCollider.enabled = true;
//         EnableHandTorch();
//     }


//     private void DisableTorchHand() {
//         // EnableInteractions();

//         controllerActivateInputAction.action.Disable();
//         controllerActivateInputAction.action.performed -= torch.OnTriggerPressed;

//         detectorCollider.enabled = false;
//         EnableFreeHand();
//     }

//     // private void EnableInteractions() {
//     //     nearFarInteractor.enableNearCasting = true;
//     //     nearFarInteractor.enableFarCasting = true;
//     // }

//     // private void DisableInteractions() {
//     //     nearFarInteractor.enableNearCasting = false;
//     //     nearFarInteractor.enableFarCasting = false;
//     // }

//     private void EnableFreeHand() {
//         handAnimator.ToggleHand(true);

//     }
//     private void EnableHandTorch() {
//         handAnimator.ToggleHand(false);
//     }

//     private void ChangeHandTorch(Side side) {
//         if (this.side == side) {
//             EnableTorchHand();
//         } else {
//             DisableTorchHand();
//         }
//     }

//     public Side GetSide() {
//         return side;
//     }

//     void Update() {
//         if (Keyboard.current[Key.V].wasPressedThisFrame) {
//             playerSettings.SetPlayerTorchHand(Side.Right);
//         }
//     }

//     void OnDisable() {
//         playerSettings.OnPlayerTorchChanged.Unregister(ChangeHandTorch);
//     }
// }
