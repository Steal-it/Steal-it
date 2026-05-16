using UnityEngine;

public class ColliderBridge : MonoBehaviour {
    private ICollisionListener listener;

    public void Initialize(ICollisionListener l) {
        listener = l;
    }

    private void OnCollisionEnter(Collision collision) {
        listener.OnCollisionEnter(collision);
    }

    private void OnCollisionExit(Collision collision) {
        listener.OnCollisionExit(collision);
    }

    private void OnTriggerEnter(Collider other) {
        listener.OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other) {
        listener.OnTriggerExit(other);
    }
}