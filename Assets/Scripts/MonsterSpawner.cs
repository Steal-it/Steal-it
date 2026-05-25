using Unity.XR.CoreUtils;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour {
    [SerializeField, Range(5f, 50f)]
    private float range = 15;
    [SerializeField, Range(1f, 30f)]
    private float respawnCooldown = 15;

    private AIMonster monster;
    private float cooldown;

    void Start() {
        cooldown = respawnCooldown;
    }

    void Update() {
        if (monster != null) return;

        cooldown -= Time.deltaTime;
        if (cooldown > 0) return;

        Collider[] playerColliderArray = Physics.OverlapSphere(transform.position, range);
        if (playerColliderArray.Length == 0) return;

        foreach (Collider playerCollider in playerColliderArray) {
            if (!playerCollider.TryGetComponent<XROrigin>(out _)) continue;

            Transform player = playerCollider.transform;
            foreach (Transform spawner in transform) {
                if (!spawner.gameObject.activeSelf) continue;

                Vector3 spawnerDirection = player.position - spawner.position;
                Vector3 playerDirection = player.forward;

                print(Vector3.Dot(spawnerDirection, playerDirection));
                if (Vector3.Dot(spawnerDirection, playerDirection) > 0) {
                    MonsterSpawnPoint spawnPoint = spawner.GetComponent<MonsterSpawnPoint>();
                    monster = spawnPoint.Spawn(spawnerDirection);

                    cooldown = respawnCooldown;
                }
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, range);

        foreach (Transform spawner in transform) {
            if (spawner.gameObject.activeSelf) {
                Gizmos.DrawWireSphere(spawner.position, 1f);
            }
        }
    }
}
