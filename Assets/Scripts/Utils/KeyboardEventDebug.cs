using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class KeyboardEventDebug : MonoBehaviour {
    [SerializeField]
    private PlayerSettingsSO playerSettings;
    [SerializeField]
    private ShakeDetector shake;
    [SerializeField]
    private SeeThrough see;
    private bool t = true;
    private bool g = false;
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
        if (Keyboard.current[Key.Z].wasPressedThisFrame) {
            g = !g;
            if (g) {

                see.EnableSeeThrough();

            } else {
                see.DisableSeeThrough();
            }
        }
    }
}
