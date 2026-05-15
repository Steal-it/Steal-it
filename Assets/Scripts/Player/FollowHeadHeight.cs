using Unity.XR.CoreUtils;
using UnityEngine;

public class FollowHeadHeight : MonoBehaviour {

    [SerializeField]
    private XROrigin originSettings;
    [SerializeField]
    private CharacterController player;
    [SerializeField]
    private Transform playerHead;
    [SerializeField]
    private float bodyMinHeight;

    private float bodyMaxHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        bodyMaxHeight = player.height;
    }

    // Update is called once per frame
    void Update() {
        float headPosition = playerHead.localPosition.y + originSettings.CameraYOffset;
        player.height = Mathf.Clamp(headPosition, bodyMinHeight, bodyMaxHeight);
        player.center = new Vector3(playerHead.localPosition.x, player.height / 2, playerHead.localPosition.z);
    }
}
