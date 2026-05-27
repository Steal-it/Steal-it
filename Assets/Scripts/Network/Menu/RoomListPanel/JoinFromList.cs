using Ubiq.Rooms;
using UnityEngine;

public class BrowseMenuControlJoinButton : MonoBehaviour {
    private BrowseMenuControl browseMenuControl;
    private RoomClient roomClient;
    private string joinCode;
    private void OnEnable() {
        browseMenuControl = GetComponentInParent<BrowseMenuControl>();
        browseMenuControl.OnBind.AddListener(BrowseRoomControl_OnBind);
    }
    private void OnDisable() {
        if (browseMenuControl) {
            browseMenuControl.OnBind.RemoveListener(BrowseRoomControl_OnBind);
        }
    }
    private void BrowseRoomControl_OnBind(RoomClient _roomClient, IRoom _room) {
        roomClient = _roomClient;
        joinCode = _room.JoinCode;
    }

    public void Join() {
        if (!roomClient || string.IsNullOrEmpty(joinCode)) {
            return;
        }
        roomClient.Join(joincode: joinCode);
    }
}
