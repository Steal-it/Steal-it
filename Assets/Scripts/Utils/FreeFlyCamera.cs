using UnityEngine;

public class FreeFlyCamera : MonoBehaviour {
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float fastMoveFactor = 3f;
    public float lookSensitivity = 3f;

    [Header("Control Settings")]
    public KeyCode fastMoveKey = KeyCode.LeftShift;
    public bool lockCursor = true;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start() {
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Initialize rotation to match current camera transform
        Vector3 rot = transform.localRotation.eulerAngles;
        rotationX = rot.y;
        rotationY = rot.x;
    }

    void Update() {
        HandleRotation();
        HandleMovement();

        // Optional: Toggle cursor lock with Escape
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
    }

    void HandleRotation() {
        rotationX += Input.GetAxis("Mouse X") * lookSensitivity;
        rotationY += Input.GetAxis("Mouse Y") * lookSensitivity;

        // Clamp vertical look to prevent flipping upside down
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
    }

    void HandleMovement() {
        float currentSpeed = moveSpeed;

        if (Input.GetKey(fastMoveKey)) {
            currentSpeed *= fastMoveFactor;
        }

        // Calculate direction based on local camera orientation
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);

        // Vertical movement (E to go up, Q to go down)
        if (Input.GetKey(KeyCode.E)) moveDirection += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) moveDirection += Vector3.down;

        transform.position += moveDirection * currentSpeed * Time.deltaTime;
    }
}