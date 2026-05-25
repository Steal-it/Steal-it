using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

public class AIMonster : MonoBehaviour {
    [SerializeField]
    private NavMeshAgent agent;

    private Transform target;

    void Start() {
        // TODO: ask to the network manager for the list of players
        target = FindAnyObjectByType<XROrigin>().transform;
    }

    void Update() {
        agent.destination = target.position;
    }
}
