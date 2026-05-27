// using System;
// using Ubiq;
// using UnityEngine;

// public class HandAnimator : MonoBehaviour {
//     [SerializeField]
//     private Animator animator;
//     [SerializeField]
//     [Tooltip("Speed at which the hand model will change grip strength in units/second. A speed of 2 will change from 0 (no grip) to 1 (full grip) in 0.5 seconds, for example. Set to 0 to disable smoothing")]
//     private float smoothingSpeed = 4;
//     [SerializeField]
//     private float sphereRadius;
//     [SerializeField]
//     private float pokeAnimationOffset;
//     [SerializeField]
//     private LayerMask pokeLayer;

//     private Side side;
//     private HeadAndHandsAvatar headAndHandsAvatar;
//     private float targetGrip;
//     private float targetPoke;
//     private float currentGrip;
//     private float currentPoke;
//     private Collider[] pokeCollision = new Collider[1];

//     private static readonly int gripProperty = Animator.StringToHash("Grabbing");
//     private static readonly int pokeProperty = Animator.StringToHash("Pokeing");
//     private static readonly int completedProperty = Animator.StringToHash("CompletedGrab");

//     void Awake() {
//         side = GetComponent<HandTorchController>().GetSide();
//         headAndHandsAvatar = GetComponentInParent<HeadAndHandsAvatar>();

//         if (!headAndHandsAvatar) {
//             Debug.LogWarning("No HeadAndHandsAvatar found among parents");
//             enabled = false;
//             return;
//         }
//     }

//     // private void Start() {
//     //     if (side == Side.Left) {
//     //         headAndHandsAvatar.OnLeftGripUpdate.AddListener(OnGripUpdate);
//     //         // headAndHandsAvatar.OnLeftHandUpdate.AddListener(OnHandUpdate);
//     //     } else {
//     //         headAndHandsAvatar.OnRightGripUpdate.AddListener(OnGripUpdate);
//     //         // headAndHandsAvatar.OnRightHandUpdate.AddListener(OnHandUpdate);
//     //     }
//     // }

//     // private void OnHandUpdate(InputVar<Pose> pose) {
//     //     bool isColliding = Physics.OverlapSphereNonAlloc(transform.position, sphereRadius, pokeCollision, pokeLayer) > 0;
//     //     if (isColliding) {
//     //         Vector3 closestPoint = pokeCollision[0].ClosestPoint(transform.position);
//     //         float pokeDistance = Vector3.Distance(transform.position, closestPoint) - pokeAnimationOffset;
//     //         targetPoke = Mathf.InverseLerp(sphereRadius, 0, pokeDistance);
//     //     } else {
//     //         targetPoke = 0;
//     //     }
//     // }

//     // private void OnGripUpdate(InputVar<float> grip) {
//     //     if (!grip.valid) {
//     //         targetGrip = 0;
//     //         return;
//     //     }
//     //     targetGrip = grip.value;

//     //     if (currentGrip == 1) {
//     //         animator.SetBool(completedProperty, true);
//     //     } else {
//     //         animator.SetBool(completedProperty, false);
//     //     }
//     // }

//     // private void Update() {
//     //     if (!Mathf.Approximately(currentGrip, targetGrip)) {
//     //         var delta = smoothingSpeed * Time.deltaTime;
//     //         currentGrip = Mathf.MoveTowards(currentGrip, targetGrip, delta);
//     //         animator.SetFloat(gripProperty, currentGrip);
//     //     }
//     //     if (!Mathf.Approximately(currentPoke, targetPoke)) {
//     //         var delta = smoothingSpeed * Time.deltaTime;
//     //         currentPoke = Mathf.MoveTowards(currentPoke, targetPoke, delta);
//     //         animator.SetFloat(pokeProperty, currentPoke);
//     //     }
//     // }

//     // public void ToggleHand(bool free) {
//     //     if (free) {
//     //         Debug.Log($"{gameObject.name} FREE HANDS");
//     //     } else {
//     //         Debug.Log($"{gameObject.name} TORCH HANDS");
//     //     }
//     // }
//     // private void OnDrawGizmosSelected() {
//     //     Gizmos.color = Color.blue;
//     //     Gizmos.DrawWireSphere(transform.position, sphereRadius);
//     // }
// }
