using UnityEngine;

public class BatteryVisuals : MonoBehaviour {

    [SerializeField]
    private Gradient emissionColor;
    [SerializeField]
    private Light orbLight;

    private Renderer orb;
    private float initialIntensity;
    void Awake() {
        orb = gameObject.GetComponentInChildren<Renderer>();
    }

    void Start() {
        Color chargeColor = emissionColor.Evaluate(0);
        orb.material.SetColor("_EmissionColor", chargeColor);
        orbLight.color = chargeColor;

        initialIntensity = orbLight.intensity;
    }

    public void UpdateVisuals(float _chargeAmount) {
        Color chargeColor = emissionColor.Evaluate(1f - _chargeAmount);
        orb.material.SetColor("_EmissionColor", chargeColor);
        orbLight.color = chargeColor;

        orbLight.intensity = _chargeAmount * initialIntensity;
    }
}
