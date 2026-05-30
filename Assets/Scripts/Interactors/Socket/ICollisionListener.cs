using UnityEngine;

public interface ICollisionListener {
    public void OnCollisionEnterRec(Collision _collision);
    public void OnCollisionExitRec(Collision _collision);
    public void OnTriggerEnterRec(Collider _other);
    public void OnTriggerExitRec(Collider _other);
}