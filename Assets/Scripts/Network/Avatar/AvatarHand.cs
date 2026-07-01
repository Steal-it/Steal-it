using System;
using Ubiq;
using Unity.XR.CoreUtils;
using UnityEngine;

public class AvatarHand : MonoBehaviour {
    [SerializeField]
    private Side side;

    private void Start() {
        XROrigin origin = FindFirstObjectByType<XROrigin>();
        if (side == Side.Left) {
            origin.transform.Find("Camera Offset/Left Controller").GetComponentInChildren<HandAnimatorController>().FreeHandAnimator = GetComponent<FreeHandAnimator>();
            origin.transform.Find("Camera Offset/Left Controller").GetComponentInChildren<HandAnimatorController>().TorchAnimator = GetComponent<TorchAnimator>();
            origin.transform.Find("Camera Offset/Left Controller").GetComponentInChildren<HandAnimatorController>().HeadAndHandsAvatar = GetComponentInParent<HeadAndHandsAvatar>();
            origin.transform.Find("Camera Offset/Left Controller").GetComponentInChildren<Torch>().OnTorchTurned += GetComponent<TorchAnimator>().ToggleLightVisual;
            GetComponent<TorchAnimator>().SetupTorch(origin.transform.Find("Camera Offset/Left Controller").GetComponentInChildren<TorchLight>());
        } else {
            origin.transform.Find("Camera Offset/Right Controller").GetComponentInChildren<HandAnimatorController>().FreeHandAnimator = GetComponent<FreeHandAnimator>();
            origin.transform.Find("Camera Offset/Right Controller").GetComponentInChildren<HandAnimatorController>().TorchAnimator = GetComponent<TorchAnimator>();
            origin.transform.Find("Camera Offset/Right Controller").GetComponentInChildren<HandAnimatorController>().HeadAndHandsAvatar = GetComponentInParent<HeadAndHandsAvatar>();
            origin.transform.Find("Camera Offset/Right Controller").GetComponentInChildren<Torch>().OnTorchTurned += GetComponent<TorchAnimator>().ToggleLightVisual;
            GetComponent<TorchAnimator>().SetupTorch(origin.transform.Find("Camera Offset/Right Controller").GetComponentInChildren<TorchLight>());
        }
    }
}
