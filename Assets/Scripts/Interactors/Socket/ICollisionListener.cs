using UnityEngine;

public interface ICollisionListener {
    public void OnCollisionEnterRec(Collision collision);
    public void OnCollisionExitRec(Collision collision);
    public void OnTriggerEnterRec(Collider other);
    public void OnTriggerExitRec(Collider other);
}