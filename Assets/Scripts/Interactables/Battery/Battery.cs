using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(NetworkObjectEnabler))]
public class Battery : MonoBehaviour {
    public event EventHandler OnBatteryRanOut;

    [SerializeField]
    private float dischargeTime = 120;
    [SerializeField]
    private float lightRangeWhenInserted;
    [SerializeField]
    private ParticleSystem runOutParticleSystem;
    [SerializeField]
    private BatteryVisuals visuals;
    [Space(5f), Header("Spring/Float Settings")]
    [Space(5f), SerializeField]
    private float spring;
    [SerializeField]
    private float damper;
    [SerializeField]
    private float restDistance;
    [SerializeField]
    private LayerMask hitMask;

    public float chargeLevel { get; private set; } = 1;
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool canFloat = true;
    private bool isInserted = false;
    private NetworkObjectEnabler networkObjectEnabler;

    private Coroutine useCoroutine;

    void Awake() {
        TryGetComponent(out rb);
        TryGetComponent(out grabInteractable);
        TryGetComponent(out networkObjectEnabler);
    }

    void Start() {
        grabInteractable.selectEntered.AddListener(DisableFloat);
        grabInteractable.selectExited.AddListener(EnableFloat);
        networkObjectEnabler.OnMessageReceived += UsgeBatteryRecived;
    }

    void FixedUpdate() {
        if (!canFloat || isInserted) return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, restDistance, hitMask)) {
            float springForce = spring * (restDistance - hit.distance);
            float dampingfactor = damper * rb.linearVelocity.y;
            float force = springForce - dampingfactor;
            rb.AddForce(Vector3.up * force);
        }
    }

    private void UsgeBatteryRecived(bool _isActive) {
        ToggleBatteryUsage(_isActive);
    }

    private void ToggleBatteryUsage(bool _use) {
        if (_use) {
            useCoroutine = StartCoroutine(UseCO());
            visuals.ChangeLightRange(lightRangeWhenInserted);
        } else {
            visuals.ChangeLightRange(null);
            if (useCoroutine != null) {
                StopCoroutine(useCoroutine);
            }
        }
    }

    public void Use() {
        ToggleBatteryUsage(true);
        networkObjectEnabler.SendEnableParameters(true);
    }

    public void StopUse() {
        ToggleBatteryUsage(false);
        networkObjectEnabler.SendEnableParameters(false);
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
            visuals.Disable();
            runOutParticleSystem.Play();

            OnBatteryRanOut?.Invoke(this, EventArgs.Empty);

            // Actaully destroy the battery after the particle system ends
            Destroy(gameObject, runOutParticleSystem.main.startLifetime.constantMax);
        }
    }

    private void DisableFloat(SelectEnterEventArgs _) {
        canFloat = false;
    }

    private void EnableFloat(SelectExitEventArgs _) {
        canFloat = true;
    }

    public void BatteryInserted() {
        rb.useGravity = false;
        isInserted = true;
        Use();
    }

    public void BatteryRemoved() {
        rb.useGravity = true;
        isInserted = false;
        StopUse();
    }

    public void Drop(Vector3 _velocity) {
        StopUse();
        canFloat = true;
        rb.AddForce(_velocity.normalized, ForceMode.Impulse);
    }

    public void Recharge(float _amount) {
        chargeLevel = Mathf.Clamp01(chargeLevel + _amount);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawRay(transform.position, Vector3.down * restDistance);
    }

    void OnDestroy() {
        grabInteractable.selectEntered.RemoveAllListeners();
    }
}
