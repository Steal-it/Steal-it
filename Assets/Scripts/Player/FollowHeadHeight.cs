using Unity.XR.CoreUtils;
using UnityEngine;

public class FollowHeadHeight : MonoBehaviour {

    [Space(10)]
    [SerializeField]
    private CharacterController player;
    [SerializeField]
    private Transform playerHead;
    [Space(10)]
    [SerializeField]
    private float bodyMinHeight;

    private float bodyMaxHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        bodyMaxHeight = player.height;
    }

    // Update is called once per frame
    void Update() {
        float headPosition = playerHead.localPosition.y;
        player.height = Mathf.Clamp(headPosition, bodyMinHeight, bodyMaxHeight);
        player.center = new Vector3(playerHead.localPosition.x, player.height / 2, playerHead.localPosition.z);
    }
}
