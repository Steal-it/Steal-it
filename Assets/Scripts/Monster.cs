using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour {
    [SerializeField]
    private NavMeshAgent agent;

    private Transform target;

    void Update() {
        if (target == null) return;

        agent.destination = target.position;
    }

    public void Initialize(Transform _playerTransform) {
        target = _playerTransform;
    }
}
