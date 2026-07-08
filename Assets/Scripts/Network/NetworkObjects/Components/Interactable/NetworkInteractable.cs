using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class NetworkInteractable : MonoBehaviour {
    private XRBaseInteractable interactable;
    private NetworkMovement networkMovement;

    public void Start() {
        interactable = gameObject.GetComponent<XRBaseInteractable>();
        networkMovement = gameObject.GetComponent<NetworkMovement>();

        if (interactable is XRGrabInteractable grabInteractable) { // it is a grab interactor
            interactable.selectEntered.AddListener(OnObjectGrabbed);
            interactable.selectExited.AddListener(OnObjectReleased);
        } else if (interactable is XRSimpleInteractable _ && TryGetComponent(out XRPokeFilter _)) { // it is a poke interactor
            interactable.hoverEntered.AddListener(OnObjectHovered);
            interactable.hoverExited.AddListener(OnObjectReleased);
        }

    }

    void OnObjectHovered(HoverEnterEventArgs _args) {
        networkMovement.SelectObject();
    }

    void OnObjectReleased(HoverExitEventArgs _args) {
        networkMovement.DeselectObject();
    }

    void OnObjectGrabbed(SelectEnterEventArgs _args) {
        networkMovement.SelectObject();
    }

    void OnObjectReleased(SelectExitEventArgs _args) {
        networkMovement.DeselectObject();
    }
}