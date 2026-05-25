using Ubiq.Rooms;
using UnityEngine;

public class BrowseMenuControlJoinButton : MonoBehaviour
{
    public BrowseMenuControl browseMenuControl;
    private RoomClient _roomClient;
    private string _joinCode;
    private void OnEnable()
    {
        browseMenuControl.OnBind.AddListener(BrowseRoomControl_OnBind);
    }
    private void OnDisable()
    {
        if (browseMenuControl)
        {
            browseMenuControl.OnBind.RemoveListener(BrowseRoomControl_OnBind);
        }
    }
    private void BrowseRoomControl_OnBind(RoomClient _roomClient, IRoom room)
    {
        this._roomClient = _roomClient;
        this._joinCode = room.JoinCode;
    }
    
    public void Join()
    {
        if (!_roomClient || string.IsNullOrEmpty(_joinCode))
        {
            return;
        }
        _roomClient.Join(joincode:_joinCode);
    }
}
