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
