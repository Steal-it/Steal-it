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
    private XRGrabInteractable grabInteractable;
    private NetworkObjectEnabler networkEnabler;

    void Awake() {
        TryGetComponent(out grabInteractable);
        TryGetComponent(out networkEnabler);
        if (networkEnabler) {
            networkEnabler.OnMessageReceived += OnNetworkEnablerMessageReceived;
        }
    }

    private void OnNetworkEnablerMessageReceived(bool _active) {
        if (!_active) {
            DisableVisuals(false);
        }
    }

    public void DisableVisuals(bool _sendToOthers) {
        if (grabInteractable != null) {
            Destroy(grabInteractable);
        }
        if (TryGetComponent(out Rigidbody rb)) {
            Destroy(rb);
        }
        if (TryGetComponent(out BoxCollider collider)) {
            Destroy(collider);
        }

        if (_sendToOthers) {
            networkEnabler.SendEnableParameters(false);
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
            NetworkReferenceManager.Instance.MessageHandler.SendAvatarComponentEnablerMessage(AvatarComponentType.Goggles, false);
        }
    }

    void OnDisable() {
        grabInteractable.selectEntered.RemoveAllListeners();
        grabInteractable.selectExited.RemoveAllListeners();
        if (networkEnabler) {
            networkEnabler.OnMessageReceived -= OnNetworkEnablerMessageReceived;
        }
    }
}
