using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HandInteractionController : MonoBehaviour {
    [SerializeField]
    private InputActionsAssociation torchInputAction;
    [SerializeField]
    private NearFarInteractor nearFarInteractor;

    public InputActionsAssociation TorchInputAction => torchInputAction;

    void Update() {
        if (!nearFarInteractor) return;
        if (nearFarInteractor.hasSelection) {
            IXRSelectInteractable genericInteractable = nearFarInteractor.interactablesSelected[0];
            if (genericInteractable is XRGrabInteractable grabInteractable && grabInteractable.TryGetComponent(out NetworkMovement netmov)) {
                netmov.SelectObject();
                grabInteractable.selectExited.AddListener(_ => { netmov.DeselectObject(); });
            }
        }
    }

    public void ToggleInteractions(bool _active) {
        nearFarInteractor.enableNearCasting = _active;
        nearFarInteractor.enableFarCasting = _active;
        torchInputAction.Init();
        torchInputAction.primaryAction.InputSetActive(!_active);
    }

    void OnDestroy() {
        torchInputAction.Disable();
    }
}
