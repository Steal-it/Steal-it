using UnityEngine;
using UnityEngine.InputSystem;

public class Torch : MonoBehaviour {
    [SerializeField]
    private float dischargeTime = 120;
    [SerializeField]
    private TorchUI torchUI;

    private float battery;

    void Start() {
        TorchManager.Instance.ControllerConfigurator.Enable(Flash);

        battery = 1;
    }

    void FixedUpdate() {
        float decrementValue = Time.fixedDeltaTime / dischargeTime;
        battery = Mathf.Clamp01(battery - decrementValue);

        torchUI.UpdateBatterySlider(battery);
    }

    private void Flash(InputAction.CallbackContext _context) {
        Debug.Log("FLASH");
    }

    void OnDestroy() {
        TorchManager.Instance.ControllerConfigurator.Disable(Flash);
    }
}
