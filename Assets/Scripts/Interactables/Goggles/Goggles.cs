using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Goggles : MonoBehaviour {
    [SerializeField]
    private float dischargeRate = 60;
    [SerializeField]
    private GameObject visuals;
    public Action<bool> OnGooglesToggle;

    private float chargeLevel = 1;
    private bool isActive = false;
    private Coroutine seeCoroutine;
    private NetworkMovement networkMovement;
    private NetworkObjectEnabler networkObjectEnabler;
    private XRGrabInteractable grabInteractable;

    void Awake() {
        TryGetComponent(out grabInteractable);
        TryGetComponent(out networkMovement);
        TryGetComponent(out networkObjectEnabler);
    }

    void Start() {
        networkObjectEnabler.OnMessageReceived += EnableGogglesVisual;
        grabInteractable.selectEntered.AddListener(Grab);
        grabInteractable.selectExited.AddListener(Release);
    }

    private void EnableGogglesVisual(bool _isActive) {
        visuals.SetActive(_isActive);
    }

    private void Release(SelectExitEventArgs arg0) {
        networkMovement.DeselectObject();
    }

    private void Grab(SelectEnterEventArgs arg0) {
        networkMovement.SelectObject();
    }

    public void DisableVisuals() {
        if (grabInteractable != null) {
            Destroy(grabInteractable);
        }
        if (TryGetComponent(out Rigidbody rb)) {
            Destroy(rb);
        }
        if (TryGetComponent(out BoxCollider collider)) {
            Destroy(collider);
        }
        networkObjectEnabler.SendEnableParameters(false);
        visuals.SetActive(false);
    }

    public void ToggleGlasses() {
        isActive = !isActive;

        if (!isActive) {
            OnGooglesToggle?.Invoke(false);
            return;
        }

        if (seeCoroutine != null) {
            StopCoroutine(seeCoroutine);
        }
        seeCoroutine = StartCoroutine(UseXray());
    }

    private IEnumerator UseXray() {
        if (chargeLevel == 0) yield break;

        OnGooglesToggle?.Invoke(true);


        while (chargeLevel > 0) {
            yield return new WaitForFixedUpdate();

            float decrementValue = Time.fixedDeltaTime / dischargeRate;
            chargeLevel = Mathf.Clamp01(chargeLevel - decrementValue);
        }

        if (chargeLevel == 0) {
            OnGooglesToggle?.Invoke(false);
        }
    }

    void OnDisable() {
        grabInteractable.selectEntered.RemoveAllListeners();
        grabInteractable.selectExited.RemoveAllListeners();
    }
}
