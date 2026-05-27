using System;
using UnityEngine;

public class HandCollisionController : MonoBehaviour {
    [SerializeField]
    private LayerMask ladderLayer;
    [SerializeField]
    private LayerMask pokeLayer;

    public event Action<bool> OnLadder;

    public event Action<float, float> OnPoke;

    private SphereCollider detectorCollider;
    private bool ladderCheck = true;

    void Awake() {
        TryGetComponent(out detectorCollider);
    }

    void OnTriggerEnter(Collider _other) {
        if (AreLayersColliding(_other, ladderLayer) && ladderCheck) {
            OnLadder?.Invoke(true);
        }
    }
    void OnTriggerStay(Collider _other) {
        if (AreLayersColliding(_other, pokeLayer)) {
            Vector3 closestPoint = _other.ClosestPoint(transform.position);
            float distanceToClosest = Vector3.Distance(transform.position, closestPoint);
            OnPoke?.Invoke(distanceToClosest, detectorCollider.radius);
        }
    }
    void OnTriggerExit(Collider _other) {
        if (AreLayersColliding(_other, ladderLayer) && ladderCheck) {
            OnLadder?.Invoke(false);
        }
        if (AreLayersColliding(_other, pokeLayer)) {
            OnPoke?.Invoke(-1, detectorCollider.radius);
        }
    }

    public void ToggleCollider(bool _active) {
        ladderCheck = _active;
    }

    private bool AreLayersColliding(Collider _other, LayerMask _layer) {
        return ((1 << _other.gameObject.layer) & _layer) != 0;
    }

}
