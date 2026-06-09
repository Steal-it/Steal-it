using System;
using System.Collections;
using UnityEngine;

public class Googles : MonoBehaviour {
    [SerializeField]
    private float dischargeTime = 60;
    [SerializeField]
    private GameObject visualsGameObject;
    public Action<bool> OnGooglesToggle;

    private float chargeLevel = 1;
    private bool isActive = false;
    private Coroutine seeCoroutine;

    public void DisableVisuals() {
        visualsGameObject.SetActive(false);
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
        seeCoroutine = StartCoroutine(UseCO());
    }

    private IEnumerator UseCO() {
        if (chargeLevel == 0) yield break;

        OnGooglesToggle?.Invoke(true);

        while (chargeLevel > 0) {
            yield return new WaitForFixedUpdate();

            float decrementValue = Time.fixedDeltaTime / dischargeTime;
            chargeLevel = Mathf.Clamp01(chargeLevel - decrementValue);
        }

        if (chargeLevel == 0) {
            OnGooglesToggle?.Invoke(false);
        }
    }
}
