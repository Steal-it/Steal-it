using UnityEngine;

public class ColliderBridge : MonoBehaviour {
    private ICollisionListener listener;

    public void Initialize(ICollisionListener l) {
        listener = l;
    }

    private void OnCollisionEnter(Collision collision) {
        listener.OnCollisionEnterRec(collision);
    }

    private void OnCollisionExit(Collision collision) {
        listener.OnCollisionExitRec(collision);
    }

    private void OnTriggerEnter(Collider other) {
        listener.OnTriggerEnterRec(other);
    }

    private void OnTriggerExit(Collider other) {
        listener.OnTriggerExitRec(other);
    }
}