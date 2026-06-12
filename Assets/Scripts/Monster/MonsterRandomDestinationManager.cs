using UnityEngine;

public class MonsterRandomDestinationManager : MonoBehaviour {
    [SerializeField]
    private float width = 50;
    [SerializeField]
    private float height = 50;

    private Vector3 minCoordinates;
    private Vector3 maxCoordinates;

    void Start() {
        minCoordinates = new Vector3(transform.position.x - width / 2, 0, transform.position.z - height / 2);
        maxCoordinates = new Vector3(transform.position.x + width / 2, 0, transform.position.z + height / 2);
    }

    public Vector3 GenerateRandomDestination() {
        float randomX = Random.Range(minCoordinates.x, maxCoordinates.x);
        float randomZ = Random.Range(minCoordinates.z, maxCoordinates.z);

        return new Vector3(randomX, 0, randomZ);
    }

    void OnDrawGizmosSelected() {
        float yOffset = 1.5f;
        Gizmos.DrawWireCube(transform.position + Vector3.up * yOffset, new Vector3(width, yOffset * 2, height));
    }
}
