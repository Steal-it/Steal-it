using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;

public class ButtonCounter : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI text;

    short pressCount = 0;

    public void IncrementCounter() {
        pressCount++;
        text.text = pressCount.ToString();
    }
}
