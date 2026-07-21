using UnityEngine;

public class Test : MonoBehaviour {
    public LayerMask layer;

    void Update() {
        // if (Physics.Raycast(transform.position, transform.forward, out var hit, 5, layer)) {
        //     print(hit.point);
        //     Debug.DrawLine(transform.position, hit.point, Color.red);
        // }
        // Debug.DrawLine(transform.position, transform.position + transform.forward * 5, Color.green);

        if (Physics.CheckSphere(transform.position, 1, layer)) {
            print("huh");
        }
    }

    // void OnDrawGizmos() {
    //     Gizmos.color = Color.green;

    //     Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5);
    // }
}
