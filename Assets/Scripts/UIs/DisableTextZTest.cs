using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class DisableTextZTest : MonoBehaviour {

    void Awake() {
        GetComponent<TMP_Text>().fontMaterial.SetInteger("unity_GUIZTestMode", (int)CompareFunction.Disabled);
    }
}
