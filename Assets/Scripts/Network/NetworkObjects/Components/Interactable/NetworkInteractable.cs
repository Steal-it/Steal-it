using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class NetworkInteractable : MonoBehaviour {
    private XRBaseInteractable interactable;
    private NetworkMovement grabbedObjectNetworkMovement;

    public void Start() {
        interactable = gameObject.GetComponent<XRBaseInteractable>();
        grabbedObjectNetworkMovement = gameObject.GetComponent<NetworkMovement>();

        interactable.selectEntered.AddListener(OnObjectGrabbed);
        interactable.selectExited.AddListener(OnObjectReleased);
    }

    void OnObjectGrabbed(SelectEnterEventArgs _args) {
        grabbedObjectNetworkMovement.SelectObject();
    }

    void OnObjectReleased(SelectExitEventArgs _args) {
        grabbedObjectNetworkMovement.DeselectObject();
    }
}