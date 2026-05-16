using UnityEngine;

public interface ICollisionListener {
    public void OnCollisionEnter(Collision collision);
    public void OnCollisionExit(Collision collision);
    public void OnTriggerEnter(Collider other);
    public void OnTriggerExit(Collider other);
}