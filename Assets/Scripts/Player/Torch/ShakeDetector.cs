using UnityEngine;
using UnityEngine.Events;

public class ShakeDetector : MonoBehaviour {
    [SerializeField]
    private float shakesVelocityThreshold = 1.5f;
    [SerializeField]
    private int shakesToTrigger;
    [SerializeField, Range(0f, 2f)]
    [Tooltip("Valid time interval to trigger a shake expressed in seconds")]
    private float shakeTimeWindow;
    [SerializeField, Range(0f, 20f)]
    private float shakeCooldown;
    [SerializeField]
    [Tooltip("Min value to register shake")]
    private float antiJitterVelocity = 1;
    [Space(10)]
    [SerializeField]
    private UnityEvent<Vector3> onShakeDetected;
    [SerializeField]
    private UnityEvent onShakeCancelled;

    private Vector3 previousVelocity;
    private Vector3 previousPosition;
    private float timeRemaining;
    private int shakeCount;
    private float cooldown;

    private void Update() {
        if (cooldown > 0) {
            cooldown -= Time.deltaTime;
        } else {
            Vector3 currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
            previousPosition = transform.position;

            if (currentVelocity.sqrMagnitude > shakesVelocityThreshold * shakesVelocityThreshold) {
                Vector3 currentDirection = currentVelocity.normalized;
                Vector3 previousDirection = previousVelocity.normalized;

                if (Vector3.Dot(currentDirection, previousDirection) < 0.5f) {
                    shakeCount++;
                    timeRemaining = shakeTimeWindow;
                    if (shakeCount == shakesToTrigger) {
                        onShakeDetected?.Invoke(currentDirection);
                        cooldown = shakeCooldown;
                        ResetShake();
                    }
                }
            }

            if (shakeCount > 0) {
                timeRemaining -= Time.deltaTime;
                if (timeRemaining <= 0) {
                    ResetShake();
                    onShakeCancelled?.Invoke();
                }
            }

            if (currentVelocity.sqrMagnitude > antiJitterVelocity * antiJitterVelocity) {
                previousVelocity = currentVelocity;
            }
        }
    }

    private void ResetShake() {
        shakeCount = 0;
        timeRemaining = 0;
    }
}
