using UnityEngine;
using UnityEngine.UI;

public class TorchUI : MonoBehaviour {
    [SerializeField]
    private Transform panelTransform;
    [SerializeField]
    private GameObject barTemplateGameObject;
    [SerializeField]
    private int barsCount = 6;
    [SerializeField]
    private Gradient gradient;

    private Camera headset;
    private Image[] barImageArray;

    void Start() {
        headset = Camera.main;

        // Correctly orientate the canvas, to constrast LookAt rotation
        transform.localScale = new Vector3(-1, 1, 1);

        // Instantiate bars
        barImageArray = new Image[barsCount];
        for (int i = 0; i < barsCount; i++) {
            GameObject barGameObject = Instantiate(barTemplateGameObject, panelTransform);
            barImageArray[i] = barGameObject.GetComponent<Image>();
        }
        barTemplateGameObject.SetActive(false);
    }

    void FixedUpdate() {
        transform.LookAt(headset.transform);
    }

    public void UpdateBatterySlider(float _value) {
        _value = Mathf.Clamp01(_value);

        // Count how many bars to display
        float barFraction = 1f / barsCount;
        int activeBarsCount = Mathf.CeilToInt(_value / barFraction);

        for (int i = 0; i < barImageArray.Length; i++) {
            Color color = gradient.Evaluate(_value);
            if (i >= activeBarsCount) {
                // Set the bar to transparent if it has not to be displayed
                color.a = 0;
            }

            barImageArray[i].color = color;
        }
    }
}
