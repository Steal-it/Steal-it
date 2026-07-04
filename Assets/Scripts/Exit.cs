using Unity.XR.CoreUtils;
using UnityEngine;

public class Exit : MonoBehaviour {
    void OnTriggerEnter(Collider _other) {
        if (_other.TryGetComponent<XROrigin>(out var _)) {
            Application.Quit();
        }
    }
}
