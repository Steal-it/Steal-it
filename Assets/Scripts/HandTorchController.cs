using UnityEngine;

public class HandTorchController : MonoBehaviour {
    [SerializeField]
    private Collider detectorCollider;
    [SerializeField]
    private GameObject standardTorchVisual;
    [SerializeField]
    private GameObject freeHandTorchVisual;
    [SerializeField]
    private Transform standardAttachPoint;
    [SerializeField]
    private Transform freeHandAttachPoint;
    [SerializeField]
    private LayerMask rungLayer;

    private TorchControllerConfigurator torchControllerConfigurator;
    private Transform torchTransform;

    void OnTriggerEnter(Collider _other) {
        // Detect if the other object is a rung of a ladder
        if (((1 << _other.gameObject.layer) & rungLayer) != 0) {
            torchControllerConfigurator.EnableInteractions();

            ActivateFreeHandTorchVisual();
        }
    }

    void OnTriggerExit(Collider _other) {
        // Detect if the other object is a rung of a ladder
        if (((1 << _other.gameObject.layer) & rungLayer) != 0) {
            torchControllerConfigurator.DisableInteractions();

            ActivateStandardTorchVisual();
        }
    }

    public void Configure(TorchControllerConfigurator _torchControllerConfigurator) {
        torchControllerConfigurator = _torchControllerConfigurator;

        // By default disable the rung detector and activate the standard hand pose
        Disable();

        standardTorchVisual.SetActive(true);
        freeHandTorchVisual.SetActive(false);
    }

    public void Enable(Transform _torchTransform) {
        torchTransform = _torchTransform;

        detectorCollider.enabled = true;
    }

    public void Disable() {
        detectorCollider.enabled = false;
    }

    private void ActivateStandardTorchVisual() {
        torchTransform.SetPositionAndRotation(standardAttachPoint.position, standardAttachPoint.rotation);

        standardTorchVisual.SetActive(true);
        freeHandTorchVisual.SetActive(false);
    }

    private void ActivateFreeHandTorchVisual() {
        torchTransform.SetPositionAndRotation(freeHandAttachPoint.position, freeHandAttachPoint.rotation);

        standardTorchVisual.SetActive(false);
        freeHandTorchVisual.SetActive(true);
    }
}
