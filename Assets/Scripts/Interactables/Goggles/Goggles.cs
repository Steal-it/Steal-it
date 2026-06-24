using System;
using System.Collections;
using UnityEngine;
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

    public void DisableVisuals() {
        Rigidbody rb = GetComponent<Rigidbody>();
        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        Collider collider = GetComponent<BoxCollider>();
        if (grab != null) {
            Destroy(grab);
        }
        if (rb != null) {
            Destroy(rb);
        }
        if (collider != null) {
            Destroy(collider);
        }
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
}
