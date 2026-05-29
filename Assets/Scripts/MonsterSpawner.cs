// using Unity.XR.CoreUtils;
// using UnityEngine;

// public class MonsterSpawner : MonoBehaviour {
//     [SerializeField, Range(5f, 50f)]
//     private float inRange = 5;
//     [SerializeField, Range(5f, 50f)]
//     private float outRange = 15;
//     [SerializeField, Range(1f, 30f)]
//     private float respawnCooldown = 15;

//     private Monster monster;
//     private float cooldown;

//     void OnValidate() {
//         if (inRange > outRange) {
//             inRange = outRange;
//         }
//     }

//     void Start() {
//         cooldown = respawnCooldown;
//     }

//     void Update() {
//         if (monster != null) return;

//         cooldown -= Time.deltaTime;
//         // Wait to try to generate a new monster
//         if (cooldown > 0) return;

//         Collider[] colliderArray = Physics.OverlapSphere(transform.position, outRange);
//         if (colliderArray.Length == 0) return;

//         foreach (Collider collider in colliderArray) {
//             if (!collider.TryGetComponent<XROrigin>(out _)) continue;

//             Transform player = collider.transform;
//             foreach (Transform spawner in transform) {
//                 if (!spawner.gameObject.activeSelf) continue;

//                 Vector3 spawnerDirection = player.position - spawner.position;
//                 Vector3 playerDirection = player.forward;

//                 if (Vector3.Dot(spawnerDirection, playerDirection) > 0) {
//                     // Vectors have similar direction, namely the player is turning his back to the spawner
//                     MonsterSpawnPoint spawnPoint = spawner.GetComponent<MonsterSpawnPoint>();
//                     monster = spawnPoint.Spawn(player);

//                     cooldown = respawnCooldown;
//                 }
//             }
//         }
//     }

//     void OnDrawGizmosSelected() {
//         Gizmos.DrawWireSphere(transform.position, outRange);

//         foreach (Transform spawner in transform) {
//             if (spawner.gameObject.activeSelf) {
//                 Gizmos.DrawWireSphere(spawner.position, 1f);
//             }
//         }
//     }
// }
