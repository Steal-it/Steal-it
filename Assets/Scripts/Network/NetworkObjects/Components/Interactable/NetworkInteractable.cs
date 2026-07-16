using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class NetworkInteractable : MonoBehaviour {
    private XRBaseInteractable interactable;
    private NetworkMovement networkMovement;

    void Awake() {
        interactable = GetComponent<XRBaseInteractable>();
        networkMovement = GetComponent<NetworkMovement>();
    }

    public void Start() {
        if (interactable is XRGrabInteractable grabInteractable) { // it is a grab interactor
            interactable.selectEntered.AddListener(OnObjectGrabbed);
            interactable.selectExited.AddListener(OnObjectReleased);
        } else if (interactable is XRSimpleInteractable _ && TryGetComponent(out XRPokeFilter _)) { // it is a poke interactor
            interactable.hoverEntered.AddListener(OnObjectHovered);
            interactable.hoverExited.AddListener(OnObjectReleased);
        }

    }

    private void OnObjectHovered(HoverEnterEventArgs _args) {
        networkMovement.SelectObject();
    }

    private void OnObjectReleased(HoverExitEventArgs _args) {
        networkMovement.DeselectObject();
    }

    private void OnObjectGrabbed(SelectEnterEventArgs _args) {
        networkMovement.SelectObject();
    }

    private void OnObjectReleased(SelectExitEventArgs _args) {
        networkMovement.DeselectObject();
    }
}