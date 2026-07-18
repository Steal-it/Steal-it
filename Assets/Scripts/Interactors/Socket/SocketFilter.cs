using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketFilter : MonoBehaviour, IXRSelectFilter, IXRHoverFilter {
    public bool canProcess => isActiveAndEnabled;
    public IReadOnlyList<XRGrabInteractable> GrabInteractableList => grabInteractableArray;

    [SerializeField]
    private XRGrabInteractable[] grabInteractableArray;

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable) {
        // Allow selection only for the specified interactables
        bool canSelect = grabInteractableArray.Contains(interactable);
        return canSelect;
    }

    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable) {
        bool canSelect = grabInteractableArray.Contains(interactable);
        return canSelect;
    }
}
