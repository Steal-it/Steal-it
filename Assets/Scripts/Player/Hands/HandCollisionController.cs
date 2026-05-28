using System;
using System.Collections.Generic;
using UnityEngine;

public class HandCollisionController : MonoBehaviour {

    private enum Trigger_phase { Enter, Stay, Exit }

    [Serializable]
    public class LayerHandler {
        public LayerMask layer;
        public bool fireOnEnter;
        public bool fireOnStay;
        public bool fireOnExit;
        public bool enabled = true;
    }

    [SerializeField]
    private LayerHandler ladderHandler;

    [SerializeField]
    private LayerHandler pokeHandler;

    public LayerHandler LadderHandler => ladderHandler;
    public LayerHandler PokeHandler => pokeHandler;

    public event Action<bool> OnLadder;
    public event Action<float, float> OnPoke;


    private SphereCollider detectorCollider;
    private Dictionary<LayerHandler, Action<Collider, Trigger_phase>> dispatch;

    void Awake() {
        TryGetComponent(out detectorCollider);

        dispatch = new() {
            [ladderHandler] = HandleLadder,
            [pokeHandler] = HandlePoke,
        };
    }

    void OnTriggerEnter(Collider _other) => Dispatch(_other, Trigger_phase.Enter);
    void OnTriggerStay(Collider _other) => Dispatch(_other, Trigger_phase.Stay);
    void OnTriggerExit(Collider _other) => Dispatch(_other, Trigger_phase.Exit);

    private void Dispatch(Collider _other, Trigger_phase _phase) {
        foreach (var (handler, action) in dispatch) {
            if (!handler.enabled) continue;
            if (!AreLayersColliding(_other, handler.layer)) continue;

            bool shouldFire = _phase switch {
                Trigger_phase.Enter => handler.fireOnEnter,
                Trigger_phase.Stay => handler.fireOnStay,
                Trigger_phase.Exit => handler.fireOnExit,
                _ => false
            };

            if (shouldFire) action(_other, _phase);
        }
    }

    private void HandleLadder(Collider _other, Trigger_phase _phase) {
        OnLadder?.Invoke(_phase == Trigger_phase.Enter);
    }

    private void HandlePoke(Collider _other, Trigger_phase _phase) {
        if (_phase == Trigger_phase.Exit) {
            OnPoke?.Invoke(-1f, detectorCollider.radius);
            return;
        }

        Vector3 closest = _other.ClosestPoint(transform.position);
        float distance = Vector3.Distance(transform.position, closest);
        OnPoke?.Invoke(distance, detectorCollider.radius);
    }

    public void SetHandlerEnabled(LayerHandler _handler, bool _active)
            => _handler.enabled = _active;

    private bool AreLayersColliding(Collider __other, LayerMask _layer) {
        return ((1 << __other.gameObject.layer) & _layer) != 0;
    }

}
