using System;
using System.Collections;
using UnityEngine;

public class Battery : MonoBehaviour {
    public event EventHandler OnBatteryRanOut;

    public float ChargeLevel => chargeLevel;

    [SerializeField]
    private float dischargeTime = 120;

    private float chargeLevel = 1;
    private bool isUsing;

    public IEnumerator Use() {
        if (chargeLevel == 0) yield break;

        isUsing = true;
        // Stop updating chargeLevel if the battery ran out or it is not used anymore
        while (chargeLevel > 0 && isUsing) {
            yield return new WaitForFixedUpdate();

            float decrementValue = Time.fixedDeltaTime / dischargeTime;
            chargeLevel = Mathf.Clamp01(chargeLevel - decrementValue);
        }

        if (chargeLevel == 0) {
            isUsing = false;
            OnBatteryRanOut?.Invoke(this, EventArgs.Empty);
        }
    }

    public void StopUse() {
        isUsing = false;
    }
}
