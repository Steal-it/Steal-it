using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlockPlayerVision : MonoBehaviour {

    [SerializeField]
    private Transform playerHead;
    [SerializeField]
    private Image visionBlockPanel;
    [Space(10)]
    [SerializeField]
    private float detectionRadius;
    [SerializeField]
    private float visionOffset;
    [SerializeField]
    private float uiDistance;
    [SerializeField]
    private LayerMask collisionLayers;
    [Space(10)]
    [SerializeField]
    private UnityEvent OnBlockVisionEnter;
    [SerializeField]
    private UnityEvent OnBlockVisionExit;

    private Collider[] hitColiders = new Collider[2];

    private bool isCurrentlyOverlapping;
    private Vector3? overlapPointNormal = null;
    private Vector3? overlapPoint = null;


    void Start() {
        if (transform.GetChild(0).TryGetComponent<Canvas>(out var canvas)) {
            if (Camera.main == null) {
                Debug.LogWarning("Main camera not found");
            }
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = uiDistance;
        }
    }

    void Update() {
        // if im overlapping once test if im forcing in the opposite direction
        if (isCurrentlyOverlapping && overlapPoint is Vector3 point && overlapPointNormal is Vector3 normal) {
            Vector3 offset = Vector3.Scale(normal, Vector3.one * visionOffset); // visionOffset
            Vector3 fromOverlap = (playerHead.position - point - offset).normalized;
            isCurrentlyOverlapping = Vector3.Dot(normal, fromOverlap) < 0; // if i force to the opposite direction im still overlapping

            if (!isCurrentlyOverlapping) {
                OnBlockVisionExit?.Invoke();
            }
        } else if (playerHead.position.y > 0.1f && Physics.OverlapSphereNonAlloc(playerHead.position, detectionRadius, hitColiders, collisionLayers) > 0) {
            float distance = ComputeClosesDistance(hitColiders);
            float alpha = Mathf.InverseLerp(detectionRadius, visionOffset, distance);
            isCurrentlyOverlapping = alpha == 1; // can overlap and if am close enough to the wall

            Fade(alpha);

            if (isCurrentlyOverlapping) {
                OnBlockVisionEnter?.Invoke();
            }

        } else {
            Fade(0);
            // reset intersection points on exit
            overlapPoint = null;
            overlapPointNormal = null;
        }
    }

    private float ComputeClosesDistance(Collider[] colliders) {
        float closestDistance = float.MaxValue;
        foreach (var col in colliders) {
            if (col != null) {
                Vector3 point = col.ClosestPoint(playerHead.position);
                float distance = Vector3.Distance(point, playerHead.position);
                closestDistance = Mathf.Min(closestDistance, distance);
                if (distance > 0 && distance < visionOffset) { // save the last intersection point
                    overlapPoint = point;
                    overlapPointNormal = (playerHead.position - point).normalized;
                }
            }
        }
        return closestDistance;
    }

    private void Fade(float alpha) {
        Color color = visionBlockPanel.color;
        color.a = alpha;
        visionBlockPanel.color = color;
    }

    private void OnDrawGizmosSelected() {
        if (playerHead == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerHead.position, detectionRadius);
    }
}
