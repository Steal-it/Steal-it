#if (UNITY_EDITOR)
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Rendering;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;

public class LadderHandlesConfigurator : MonoBehaviour {
    [SerializeField]
    [Tooltip("Collider of the entire ladder to get its height")]
    private BoxCollider ladder;
    [Space(10)]
    [SerializeField, Min(0)]
    private int handleCount = 1;
    [SerializeField, Min(0)]
    [Tooltip("Amount of space before and after the handles")]
    private float padding;
    [Space(10)]
    [SerializeField]
    private GameObject handlePrefab;
    [Space(10)]
#pragma warning disable CS0618 // Type or member is obsolete
    [SerializeField]
    private XRInteractableAffordanceStateProvider affordanceProvider;
#pragma warning restore CS0618 // Type or member is obsolete

    private int previousHandleCount;
    private float previousPadding;

    private void OnValidate() {

        if (Application.isPlaying) return;

        if (handlePrefab == null || ladder == null || handleCount < 1) return;

#if (UNITY_EDITOR)
        if (previousHandleCount != handleCount || previousPadding != padding) {
            EditorApplication.delayCall -= () => DisplaceHandles();
            EditorApplication.delayCall += () => DisplaceHandles();
        }
#endif

        previousHandleCount = handleCount;
        previousPadding = padding;
    }

#if (UNITY_EDITOR)
    private void DisplaceHandles() {
        if (this == null) return;

        ClearHandles();

        float ladderHeight = ladder.size.y - (padding * 2); // remove padding
        float handleOffset = ladderHeight / Mathf.Max(1, (handleCount - 1));

        // apply the bottom offset
        transform.position = new Vector3(transform.position.x, ladder.size.y / 2, transform.position.z);

        for (int i = 0; i < handleCount; i++) {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(handlePrefab);
#pragma warning disable CS0618 // Type or member is obsolete
            if (instance && instance.transform.GetChild(1).TryGetComponent<ColorMaterialPropertyAffordanceReceiver>(out var affordance)) {
#pragma warning restore CS0618 // Type or member is obsolete
                affordance.affordanceStateProvider = affordanceProvider;
                instance.transform.SetParent(transform.GetChild(0));
                Vector3 pos = this.transform.position + new Vector3(0, (i * handleOffset) - ladderHeight / 2, 0);
                instance.transform.position = pos;
            }
        }
    }
#endif

    private void ClearHandles() {
        Transform child = transform.GetChild(0);
        while (child.childCount > 0) DestroyImmediate(child.GetChild(0).gameObject);
    }
}
