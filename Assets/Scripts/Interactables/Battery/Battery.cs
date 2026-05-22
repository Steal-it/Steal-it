using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Battery : MonoBehaviour {
    public event EventHandler OnBatteryRanOut;

    [SerializeField]
    private float dischargeTime = 120;
    [SerializeField]
    private ParticleSystem runOutParticleSystem;
    [SerializeField]
    private GameObject visualsGameObject;
    [SerializeField]
    private BatteryUI batteryUI;

    private XRGrabInteractable grabInteractable;
    private float chargeLevel = 1;
    private bool isUsing;

    void Start() {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(EnableUI);
        grabInteractable.selectExited.AddListener(DisableUI);
    }

    public void Use() {
        EnableUI(null);

        StartCoroutine(UseCO());
    }

    public void StopUse() {
        isUsing = false;
    }

    public void DisableUI(SelectExitEventArgs _event) {
        batteryUI.ToggleDisplay(false);
    }

    private void EnableUI(SelectEnterEventArgs _event) {
        batteryUI.ToggleDisplay(true);
    }

    private IEnumerator UseCO() {
        if (chargeLevel == 0) yield break;

        isUsing = true;
        // Stop updating chargeLevel if the battery ran out or it is not used anymore
        while (chargeLevel > 0 && isUsing) {
            yield return new WaitForFixedUpdate();

            float decrementValue = Time.fixedDeltaTime / dischargeTime;
            chargeLevel = Mathf.Clamp01(chargeLevel - decrementValue);

            batteryUI.UpdateBatteryDisplay(chargeLevel);
        }

        if (chargeLevel == 0) {
            // Logically stop and visually destroy the battery
            isUsing = false;
            runOutParticleSystem.Play();
            visualsGameObject.SetActive(false);

            OnBatteryRanOut?.Invoke(this, EventArgs.Empty);

            // Actaully destroy the battery after the particle system ends
            Destroy(gameObject, runOutParticleSystem.main.startLifetime.constantMax);
        }
    }

    void OnDestroy() {
        grabInteractable.selectEntered.RemoveAllListeners();
        grabInteractable.selectExited.RemoveAllListeners();
    }
}
