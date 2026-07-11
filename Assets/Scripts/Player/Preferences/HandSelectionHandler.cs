using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HandSelectionHandler : MonoBehaviour {
    [SerializeField]
    private PlayerSettingsSO playerSettings;
    [SerializeField]
    private Button leftHandButton;
    [SerializeField]
    private Button rightHandButton;
    [SerializeField]
    private Renderer leftHandButtonLight;
    [SerializeField]
    private Renderer rightHandButtonLight;
    [SerializeField]
    private Material buttonSelectedMaterial;
    [SerializeField]
    private Material buttonDeselectedMaterial;

    void Awake() {
        rightHandButton.onClick.AddListener(OnSelectRightHandButtonClicked);
        leftHandButton.onClick.AddListener(OnSelectLeftHandButtonClicked);

        if (playerSettings.playerTorchHand == Side.Left) {
            rightHandButtonLight.material = buttonDeselectedMaterial;
            leftHandButtonLight.material = buttonSelectedMaterial;
        } else {
            rightHandButtonLight.material = buttonSelectedMaterial;
            leftHandButtonLight.material = buttonDeselectedMaterial;
        }
    }

    private void OnSelectRightHandButtonClicked() {
        if (playerSettings.playerTorchHand != Side.Right) {
            rightHandButtonLight.material = buttonSelectedMaterial;
            leftHandButtonLight.material = buttonDeselectedMaterial;
            playerSettings.SetPlayerTorchHand(Side.Right);
        }
    }

    private void OnSelectLeftHandButtonClicked() {
        if (playerSettings.playerTorchHand != Side.Left) {
            rightHandButtonLight.material = buttonDeselectedMaterial;
            leftHandButtonLight.material = buttonSelectedMaterial;
            playerSettings.SetPlayerTorchHand(Side.Left);
        }
    }

    void OnDestroy() {
        rightHandButton.onClick.RemoveListener(OnSelectRightHandButtonClicked);
        leftHandButton.onClick.RemoveListener(OnSelectLeftHandButtonClicked);
    }
}
