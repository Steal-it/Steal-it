using UnityEngine;

public class BlockPlayerVision : MonoBehaviour {

    private const string COLOR_PROPERTY = "_BaseColor";

    [SerializeField]
    private Transform playerHead;
    [SerializeField]
    private float detectionRadius;
    [SerializeField]
    private float visionOffset;
    [SerializeField]
    private LayerMask collisionLayers;

    private Renderer visionBlockRenderer;
    void Awake() {
        visionBlockRenderer = GetComponent<MeshRenderer>();
        visionBlockRenderer.enabled = true;
    }

    void Update() {
        Collider[] hitColiders = Physics.OverlapSphere(playerHead.position, detectionRadius, collisionLayers);
        if (hitColiders.Length > 0) {
            float distance = ComputeClosesDistance(hitColiders);
            float alpha = Mathf.InverseLerp(detectionRadius, 0.05f, distance);
            Fade(alpha);
        } else {
            Fade(0);
        }
    }

    private float ComputeClosesDistance(Collider[] colliders) {
        float closestDistance = float.MaxValue;
        foreach (var col in colliders) {
            print(col.gameObject);
            float distance = Vector3.Distance(col.ClosestPoint(playerHead.position), playerHead.position);
            closestDistance = Mathf.Min(closestDistance, distance);
        }
        return closestDistance;
    }

    private void Fade(float alpha) {
        Color color = visionBlockRenderer.material.GetColor(COLOR_PROPERTY);
        color.a = alpha;
        visionBlockRenderer.material.SetColor(COLOR_PROPERTY, color);

        visionBlockRenderer.enabled = alpha > 0;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerHead.position, detectionRadius);
    }
}
