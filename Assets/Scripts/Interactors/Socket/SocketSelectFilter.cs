using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketSelectFilter : MonoBehaviour, IXRSelectFilter {
  [SerializeField]
  private XRGrabInteractable[] GrabInteractableArray;

  public bool canProcess => isActiveAndEnabled;

  public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable) {
    // Allow selection only for the specified interactables
    bool canSelect = GrabInteractableArray.Contains(interactable);
    return canSelect;
  }
}
