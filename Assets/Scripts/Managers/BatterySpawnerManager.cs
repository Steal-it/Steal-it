using UnityEngine;

public class BatterySpawnerManager : MonoBehaviour {
    [SerializeField]
    private GameObject batteryPrefabGameObject;

    private void Start() {
        foreach (Transform spawner in transform) {
            if (spawner.gameObject.activeSelf) {
                Battery battery = Instantiate(batteryPrefabGameObject, spawner.position, spawner.rotation, spawner).GetComponent<Battery>();
            }
        }
    }

    void OnDrawGizmosSelected() {
        foreach (Transform spawner in transform) {
            if (spawner.gameObject.activeSelf) {
                Gizmos.DrawWireSphere(spawner.position, 0.05f);
            }
        }
    }
}
