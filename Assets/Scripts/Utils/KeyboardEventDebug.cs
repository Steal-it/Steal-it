using UnityEngine;
using UnityEngine.InputSystem;


public class KeyboardEventDebug : MonoBehaviour {
    [SerializeField]
    private PlayerSettingsSO playerSettings;
    [SerializeField]
    private ShakeDetector shake;
    private bool t = true;
    void Update() {
        if (Keyboard.current[Key.V].wasPressedThisFrame) {
            if (t) {
                playerSettings.SetPlayerTorchHand(Side.Right);
                t = false;
            } else {
                playerSettings.SetPlayerTorchHand(Side.Left);
                t = true;
            }
        }

        if (Keyboard.current[Key.X].wasPressedThisFrame) {
            shake.ToggleShake(true);
        }
    }
}
