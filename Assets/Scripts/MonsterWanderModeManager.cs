using UnityEngine;

public class MonsterWanderModeManager : MonoBehaviour {
    [SerializeField]
    private float width;
    [SerializeField]
    private float height;
    // [SerializeField]
    // private Monster monster;

    private Vector3 minCoordinates;
    private Vector3 maxCoordinates;

    void Start() {
        // monster.OnAgentStopped += Monster_OnAgentStopped;

        minCoordinates = new Vector3(transform.position.x - width / 2, 0, transform.position.z - height / 2);
        maxCoordinates = new Vector3(transform.position.x + width / 2, 0, transform.position.z + height / 2);

        // GenerateRandomDestinationToAgent();
    }

    // private void Monster_OnAgentStopped(object _sender, System.EventArgs _event) {
    //     GenerateRandomDestinationToAgent();
    // }

    public Vector3 GenerateRandomDestinationToAgent() {
        float randomX = Random.Range(minCoordinates.x, maxCoordinates.x);
        float randomZ = Random.Range(minCoordinates.z, maxCoordinates.z);

        return new Vector3(randomX, 0, randomZ);

        // Vector3 randomDestination = new Vector3(randomX, 0, randomZ);

        // monster.SetDestination(randomDestination);
    }

    void OnDrawGizmosSelected() {
        float yOffset = 1.5f;
        Gizmos.DrawWireCube(transform.position + Vector3.up * yOffset, new Vector3(width, yOffset * 2, height));
    }

    // void OnDestroy() {
    //     monster.OnAgentStopped -= Monster_OnAgentStopped;
    // }
}
