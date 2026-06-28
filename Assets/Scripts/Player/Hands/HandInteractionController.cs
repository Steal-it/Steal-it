using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HandInteractionController : MonoBehaviour {
    [SerializeField]
    private InputActionsAssociation torchInputAction;

    public InputActionsAssociation TorchInputAction => torchInputAction;

    private NearFarInteractor nearFarInteractor;

    public void Init(Side _side) {
        XROrigin origin = FindFirstObjectByType<XROrigin>();
        if (_side == Side.Left) {
            nearFarInteractor = origin.transform.Find("Camera Offset/Left Controller").GetComponentInChildren<NearFarInteractor>();
        } else {
            nearFarInteractor = origin.transform.Find("Camera Offset/Right Controller").GetComponentInChildren<NearFarInteractor>();
        }
    }

    void Update() {
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
