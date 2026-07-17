using UnityEngine;
using UnityEngine.UI;

public class HandSelectionHandler : MonoBehaviour {
    [SerializeField]
    private PlayerSettingsSO playerSettings;
    [SerializeField]
    private Button leftButton;
    [SerializeField]
    private GameObject leftOutline;
    [SerializeField]
    private Button rightButton;
    [SerializeField]
    private GameObject rightOutline;

    void Awake() {
        leftButton.onClick.AddListener(OnSelectLeftHandButtonClicked);
        rightButton.onClick.AddListener(OnSelectRightHandButtonClicked);

        if (playerSettings.playerTorchHand == Side.Left) {
            SelectLeftButton();
        } else {
            SelectRightButton();
        }
    }

    private void OnSelectLeftHandButtonClicked() {
        if (playerSettings.playerTorchHand != Side.Left) {
            playerSettings.playerTorchHand = Side.Left;

            SelectLeftButton();
        }
    }

    private void OnSelectRightHandButtonClicked() {
        if (playerSettings.playerTorchHand != Side.Right) {
            playerSettings.playerTorchHand = Side.Right;

            SelectRightButton();
        }
    }

    private void SelectLeftButton() {
        leftOutline.SetActive(true);
        rightOutline.SetActive(false);
    }

    private void SelectRightButton() {
        leftOutline.SetActive(false);
        rightOutline.SetActive(true);
    }

    void OnDestroy() {
        rightButton.onClick.RemoveListener(OnSelectRightHandButtonClicked);
        leftButton.onClick.RemoveListener(OnSelectLeftHandButtonClicked);
    }
}
