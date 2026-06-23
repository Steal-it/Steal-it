using System;
using System.Collections;
using UnityEngine;
public class Battery : MonoBehaviour {
    public event EventHandler OnBatteryRanOut;

    [SerializeField]
    private float dischargeTime = 120;
    [SerializeField]
    private ParticleSystem runOutParticleSystem;
    [SerializeField]
    private BatteryVisuals visuals;

    public float chargeLevel { get; private set; } = 1;
    private Rigidbody rb;

    private Coroutine useCoroutine;

    void Awake() {
        TryGetComponent(out rb);
    }

    public void Use() {
        useCoroutine = StartCoroutine(UseCO());
    }

    public void StopUse() {
        if (useCoroutine != null) {
            StopCoroutine(useCoroutine);
        }
    }

    private IEnumerator UseCO() {
        if (chargeLevel == 0) yield break;

        // Stop updating chargeLevel if the battery ran out or it is not used anymore
        while (chargeLevel > 0) {
            yield return new WaitForFixedUpdate();

            float decrementValue = Time.fixedDeltaTime / dischargeTime;
            chargeLevel = Mathf.Clamp01(chargeLevel - decrementValue);

            visuals.UpdateVisuals(chargeLevel);
        }

        if (chargeLevel == 0) {
            // Logically stop and visually destroy the battery
            visuals.enabled = false;
            runOutParticleSystem.Play();

            OnBatteryRanOut?.Invoke(this, EventArgs.Empty);

            // Actaully destroy the battery after the particle system ends
            Destroy(gameObject, runOutParticleSystem.main.startLifetime.constantMax);
        }
    }

    public void Drop(Vector3 _velocity) {
        StopUse();
        rb.useGravity = true;
        rb.AddForce(_velocity.normalized, ForceMode.Impulse);
    }

    public void Recharge(float _amount) {
        chargeLevel = _amount;
    }
}
