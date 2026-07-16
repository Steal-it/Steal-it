using UnityEngine;

public class BatteryVisuals : MonoBehaviour {

    [SerializeField]
    private Gradient emissionColor;
    [SerializeField]
    private Light orbLight;

    private Renderer orb;
    private float initialIntensity;
    private float initialRange;
    void Awake() {
        orb = gameObject.GetComponentInChildren<Renderer>();
    }

    void Start() {
        Color chargeColor = emissionColor.Evaluate(0);
        orb.material.SetColor("_EmissionColor", chargeColor);
        orbLight.color = chargeColor;

        initialIntensity = orbLight.intensity;
        initialRange = orbLight.range;
    }

    public void ChangeLightRange(float? lightRange) {
        orbLight.range = lightRange ?? initialRange;
    }

    public void UpdateVisuals(float _chargeAmount) {
        Color chargeColor = emissionColor.Evaluate(1f - _chargeAmount);
        orb.material.SetColor("_EmissionColor", chargeColor);
        orbLight.color = chargeColor;

        orbLight.intensity = _chargeAmount * initialIntensity;
    }

    public void Disable() {
        orb.enabled = false;
        orbLight.enabled = false;
    }

    public void Enable() {
        orb.enabled = true;
        orbLight.enabled = true;
    }

}
