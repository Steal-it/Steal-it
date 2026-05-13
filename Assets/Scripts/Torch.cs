using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Torch : MonoBehaviour {
    [SerializeField]
    private InputActionReference inputAction;

    void Awake() {
        inputAction.action.Enable();
        inputAction.action.performed += Flash;
        // TODO: track device change https://youtu.be/NObwdF9RqCg?si=QtYNqLSqe-xpNHuo
    }

    private void Flash(InputAction.CallbackContext context) {
        Debug.Log("FLASH");
    }

    void OnDestroy() {
        inputAction.action.Disable();
        inputAction.action.performed -= Flash;
    }
}
