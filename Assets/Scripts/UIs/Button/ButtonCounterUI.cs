using TMPro;
using UnityEngine;

public class ButtonCounterUI : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI text;

    public void IncrementCounterUI(short value) {
        text.text = value.ToString();
    }
}
