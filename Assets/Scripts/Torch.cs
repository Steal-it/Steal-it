using UnityEngine;
using UnityEngine.InputSystem;

public class Torch : MonoBehaviour {
    void Start() {
        TorchManager.Instance.ControllerConfigurator.Enable(Flash);
    }

    private void Flash(InputAction.CallbackContext _context) {
        Debug.Log("FLASH");
    }

    void OnDestroy() {
        TorchManager.Instance.ControllerConfigurator.Disable(Flash);
    }
}
