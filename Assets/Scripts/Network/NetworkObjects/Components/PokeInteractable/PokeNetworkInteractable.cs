using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PokeNetworkInteractable : MonoBehaviour {
    private XRBaseInteractable interactable;

    private NetworkMovement grabbedObjectNetworkMovement;

    public void Start() {
        interactable = gameObject.GetComponent<XRBaseInteractable>();
        grabbedObjectNetworkMovement = gameObject.GetComponent<NetworkMovement>();

        interactable.hoverEntered.AddListener(OnObjectHovered);
        interactable.hoverExited.AddListener(OnObjectReleased);
    }

    void OnObjectHovered(HoverEnterEventArgs _args) {
        grabbedObjectNetworkMovement.SelectObject();
    }

    void OnObjectReleased(HoverExitEventArgs _args) {
        grabbedObjectNetworkMovement.DeselectObject();
    }
}