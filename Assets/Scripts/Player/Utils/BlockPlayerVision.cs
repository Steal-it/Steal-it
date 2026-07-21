using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlockPlayerVision : MonoBehaviour
{

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


    void Start()
    {
        if (transform.GetChild(0).TryGetComponent<Canvas>(out var canvas))
        {
            if (Camera.main == null)
            {
                Debug.LogWarning("Main camera not found");
            }
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = uiDistance;
        }
    }

    void Update()
    {
        // if im overlapping once test if im forcing in the opposite direction
        if (isCurrentlyOverlapping && overlapPoint is Vector3 point && overlapPointNormal is Vector3 normal)
        {
            Vector3 offset = Vector3.Scale(normal, Vector3.one * visionOffset); // visionOffset
            Vector3 fromOverlap = (playerHead.position - point - offset).normalized;
            isCurrentlyOverlapping = Vector3.Dot(normal, fromOverlap) < 0; // if i force to the opposite direction im still overlapping

            if (!isCurrentlyOverlapping)
            {
                OnBlockVisionExit?.Invoke();
            }
        }
        else if (playerHead.position.y > 0.1f && Physics.OverlapSphereNonAlloc(playerHead.position, detectionRadius, hitColiders, collisionLayers) > 0)
        {
            float distance = ComputeClosestDistance(hitColiders);
            float alpha = Mathf.InverseLerp(detectionRadius, visionOffset, distance);
            isCurrentlyOverlapping = alpha == 1; // can overlap and if am close enough to the wall
            Fade(alpha);

            if (isCurrentlyOverlapping)
            {
                OnBlockVisionEnter?.Invoke();
            }

        }
        else
        {
            Fade(0);
            // reset intersection points on exit
            overlapPoint = null;
            overlapPointNormal = null;
        }
    }

    private float ComputeClosestDistance(Collider[] colliders)
    {
        float closestDistance = float.MaxValue;
        foreach (var col in colliders)
        {
            if (col != null)
            {
                Vector3 point;
                if (col is MeshCollider mcol)
                {
                    point = ClosestPointOnMesh(mcol, playerHead.position);
                }
                else
                {
                    point = col.ClosestPoint(playerHead.position);
                }
                //Vector3 point = col.ClosestPoint(playerHead.position);
                float distance = Vector3.Distance(point, playerHead.position);
                closestDistance = Mathf.Min(closestDistance, distance);
                if (distance > 0 && distance < visionOffset)
                { // save the last intersection point
                    overlapPoint = point;
                    overlapPointNormal = (playerHead.position - point).normalized;
                }
            }
        }
        return closestDistance;
    }

    private static Vector3 ClosestPointOnMesh(MeshCollider meshCollider, Vector3 worldPoint)
    {
        Mesh mesh = meshCollider.sharedMesh;
        Transform t = meshCollider.transform;

        Vector3 localPoint = t.InverseTransformPoint(worldPoint);

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        float closestSqrDist = float.MaxValue;
        Vector3 closestPoint = localPoint;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 a = vertices[triangles[i]];
            Vector3 b = vertices[triangles[i + 1]];
            Vector3 c = vertices[triangles[i + 2]];

            Vector3 p = ClosestPointOnTriangle(localPoint, a, b, c);
            float sqrDist = (p - localPoint).sqrMagnitude;

            if (sqrDist < closestSqrDist)
            {
                closestSqrDist = sqrDist;
                closestPoint = p;
            }
        }

        return t.TransformPoint(closestPoint);
    }

    private static Vector3 ClosestPointOnTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        Vector3 ap = p - a;

        float d1 = Vector3.Dot(ab, ap);
        float d2 = Vector3.Dot(ac, ap);
        if (d1 <= 0f && d2 <= 0f) return a;

        Vector3 bp = p - b;
        float d3 = Vector3.Dot(ab, bp);
        float d4 = Vector3.Dot(ac, bp);
        if (d3 >= 0f && d4 <= d3) return b;

        float vc = d1 * d4 - d3 * d2;
        if (vc <= 0f && d1 >= 0f && d3 <= 0f)
        {
            float v = d1 / (d1 - d3);
            return a + v * ab;
        }

        Vector3 cp = p - c;
        float d5 = Vector3.Dot(ab, cp);
        float d6 = Vector3.Dot(ac, cp);
        if (d6 >= 0f && d5 <= d6) return c;

        float vb = d5 * d2 - d1 * d6;
        if (vb <= 0f && d2 >= 0f && d6 <= 0f)
        {
            float w = d2 / (d2 - d6);
            return a + w * ac;
        }

        float va = d3 * d6 - d5 * d4;
        if (va <= 0f && (d4 - d3) >= 0f && (d5 - d6) >= 0f)
        {
            float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
            return b + w * (c - b);
        }

        float denom = 1f / (va + vb + vc);
        float v2 = vb * denom;
        float w2 = vc * denom;
        return a + ab * v2 + ac * w2;
    }

    private void Fade(float alpha)
    {
        Color color = visionBlockPanel.color;
        color.a = alpha;
        visionBlockPanel.color = color;
    }

    private void OnDrawGizmosSelected()
    {
        if (playerHead == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerHead.position, detectionRadius);
    }
}
