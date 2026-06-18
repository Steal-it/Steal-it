using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;

public class SpectatorMode : MonoBehaviour {
    [SerializeField]
    private XROrigin rig;
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private GravityProvider gravityProvider;
    [SerializeField, Range(2, 4)]
    private float height = 3;

    public bool enable;

    void Update() {
        if (enable) {
            Enable();
        } else {
            Disable();
        }
    }

    private void Enable() {
        Vector3 position = rig.transform.position;
        position.y = height;
        rig.transform.position = position;

        characterController.enabled = false;
        gravityProvider.enabled = false;
    }

    private void Disable() {
        Vector3 position = rig.transform.position;
        position.y = 0;
        rig.transform.position = position;

        characterController.enabled = true;
        gravityProvider.enabled = true;
    }
}