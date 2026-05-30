using UnityEngine;

public class TorchLight : MonoBehaviour {
    private Light torchLight;
    private bool power;

    void Awake() {
        TryGetComponent(out torchLight);
    }

    public void ToggleLight(object _, Torch.OnTorchTurnedEventArgs _eventArgs) {
        torchLight.enabled = _eventArgs.isTurnedOn;
        power = torchLight.enabled;
    }

    public void InPocket(bool _isInPocket) {
        if (_isInPocket) {
            torchLight.enabled = false;
        } else {
            torchLight.enabled = power;
        }
    }

}
