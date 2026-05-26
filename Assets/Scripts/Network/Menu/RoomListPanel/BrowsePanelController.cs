using System.Collections.Generic;
using Ubiq.Rooms;
using Ubiq.XR.Notifications;
using UnityEngine;
using TMPro;
using System;

public class BrowsePanelController : MonoBehaviour
{
    private float roomRefreshInterval = 2.0f;
    [SerializeField]
    private Menu mainMenu;
    [SerializeField]
    private Transform controlsRoot;
    [SerializeField]
    private GameObject controlTemplate;
    [SerializeField]
    private TextMeshProUGUI connectionStatusText;
    private List<BrowseMenuControl> controls = new List<BrowseMenuControl>();
    private List<IRoom> lastRoomArgs;
    private float nextRoomRefreshTime = -1;
    private void OnEnable()
    {
        mainMenu.RoomClient.OnRooms.AddListener(RoomClient_OnRoomsDiscovered);
        mainMenu.RoomClient.OnJoinedRoom.AddListener(RoomClient_OnJoinedRoom);
        PlayerNotifications.OnNotification += UpdateConnectionStatus;
        UpdateAvailableRooms();
    }
    private void OnDisable()
    {
        if (mainMenu.RoomClient)
        {
            mainMenu.RoomClient.OnRooms.RemoveListener(RoomClient_OnRoomsDiscovered);
            mainMenu.RoomClient.OnJoinedRoom.RemoveListener(RoomClient_OnJoinedRoom);
        }
    }
    private BrowseMenuControl InstantiateControl (bool _isRoomTheCurrentRoom) {
        var go = GameObject.Instantiate(controlTemplate, controlsRoot);
        if(!_isRoomTheCurrentRoom)
        {
            go.SetActive(true);
        }
        return go.GetComponent<BrowseMenuControl>();
    }
    private void RoomClient_OnJoinedRoom(IRoom _room)
    {
        UpdateAvailableRooms();
        // Immediately ask for a refresh - maybe room we left is now empty
        mainMenu.RoomClient.DiscoverRooms();
    }
    private void RoomClient_OnRoomsDiscovered(List<IRoom> _rooms,RoomsDiscoveredRequest _request)
    {
        // Ignore filtered requests
        if (string.IsNullOrEmpty(_request.joincode))
        {
            lastRoomArgs = _rooms;
            UpdateAvailableRooms();
        }
    }
    private void UpdateAvailableRooms() {
        var rooms = lastRoomArgs;
        if (rooms == null || rooms.Count == 0)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                Destroy(controls[i].gameObject);
            }
            controls.Clear();

            return;
        }

        this.gameObject.SetActive(true);
        int controlI = 0;

        for (int roomI = 0; roomI < rooms.Count; controlI++,roomI++)
        {
            bool isRoomTheCurrentRoom = rooms[roomI].Name == mainMenu.RoomClient.Room.Name;

            if (controls.Count <= controlI) {
                controls.Add(InstantiateControl(isRoomTheCurrentRoom));
            }

            controls[controlI].Bind(mainMenu.RoomClient,rooms[roomI]);

            if(isRoomTheCurrentRoom) {
                connectionStatusText.text = "Connected";
            }

        }
        while (controls.Count > controlI) {
            Destroy(controls[controlI].gameObject);
            controls.RemoveAt(controlI);
        }
    }
    private void Update()
    {
        if (Time.realtimeSinceStartup > nextRoomRefreshTime)
        {
            mainMenu.RoomClient.DiscoverRooms();
            nextRoomRefreshTime = Time.realtimeSinceStartup + roomRefreshInterval;
        }
    }

    private void UpdateConnectionStatus(Notification _notification) {
        if(_notification.Message.Contains("Connection lost")) {
            connectionStatusText.text = "Currently not connected to any room (connection lost)";
        }
    }
}