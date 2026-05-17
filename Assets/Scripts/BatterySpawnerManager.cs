using UnityEngine;

public class BatterySpawnerManager : MonoBehaviour {
    [SerializeField]
    private GameObject batteryPrefabGameObject;

    private void Start() {
        foreach (Transform spawner in transform) {
            Instantiate(batteryPrefabGameObject, spawner.position, spawner.rotation, spawner);
        }
    }

    void OnDrawGizmosSelected() {
        foreach (Transform spawner in transform) {
            Gizmos.DrawWireSphere(spawner.position, 0.05f);
        }
    }
}
