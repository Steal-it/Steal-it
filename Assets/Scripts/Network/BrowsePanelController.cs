using System.Collections.Generic;
using Ubiq.Rooms;
using Ubiq.XR.Notifications;
using UnityEngine;
using TMPro;

public class BrowsePanelController : MonoBehaviour
{
    public float roomRefreshInterval = 2.0f;
    public Menu mainMenu;
    public Transform controlsRoot;
    public GameObject roomListPanel;
    public GameObject controlTemplate;
    public TextMeshProUGUI connectionStatusText;
    private List<BrowseMenuControl> controls = new List<BrowseMenuControl>();
    private List<IRoom> lastRoomArgs;
    private float nextRoomRefreshTime = -1;
    private void OnEnable()
    {
        mainMenu.roomClient.OnRooms.AddListener(RoomClient_OnRoomsDiscovered);
        mainMenu.roomClient.OnJoinedRoom.AddListener(RoomClient_OnJoinedRoom);
        PlayerNotifications.OnNotification += UpdateConnectionStatus;
        UpdateAvailableRooms();
    }
    private void OnDisable()
    {
        if (mainMenu.roomClient)
        {
            mainMenu.roomClient.OnRooms.RemoveListener(RoomClient_OnRoomsDiscovered);
            mainMenu.roomClient.OnJoinedRoom.RemoveListener(RoomClient_OnJoinedRoom);
        }
    }
    private BrowseMenuControl InstantiateControl () {
        var go = GameObject.Instantiate(controlTemplate, controlsRoot);
        go.SetActive(true);
        return go.GetComponent<BrowseMenuControl>();
    }
    private void RoomClient_OnJoinedRoom(IRoom room)
    {
        UpdateAvailableRooms();
        // Immediately ask for a refresh - maybe room we left is now empty
        mainMenu.roomClient.DiscoverRooms();
    }
    private void RoomClient_OnRoomsDiscovered(List<IRoom> rooms,RoomsDiscoveredRequest request)
    {
        // Ignore filtered requests
        if (string.IsNullOrEmpty(request.joincode))
        {
            lastRoomArgs = rooms;
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

        roomListPanel.SetActive(true);
        int controlI = 0;

        for (int roomI = 0; roomI < rooms.Count; controlI++,roomI++)
        {
            bool isRoomTheCurrentRoom = rooms[roomI].Name == mainMenu.roomClient.Room.Name;

            if (controls.Count <= controlI) {
                controls.Add(InstantiateControl());
            }

            controls[controlI].Bind(mainMenu.roomClient,rooms[roomI]);

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
            mainMenu.roomClient.DiscoverRooms();
            nextRoomRefreshTime = Time.realtimeSinceStartup + roomRefreshInterval;
        }
    }

    private void UpdateConnectionStatus(Notification test) {
        if(test.Message.Contains("Connection lost")) {
            connectionStatusText.text = "Currently not connected to any room (connection lost)";
        }
    }
}