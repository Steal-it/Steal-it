using UnityEngine;

public class ColliderBridge : MonoBehaviour {
    private ICollisionListener listener;

    public void Initialize(ICollisionListener _listener) {
        listener = _listener;
    }

    private void OnCollisionEnter(Collision _collision) {
        listener.OnCollisionEnterRec(_collision);
    }

    private void OnCollisionExit(Collision _collision) {
        listener.OnCollisionExitRec(_collision);
    }

    private void OnTriggerEnter(Collider _other) {
        listener.OnTriggerEnterRec(_other);
    }

    private void OnTriggerExit(Collider _other) {
        listener.OnTriggerExitRec(_other);
    }
}