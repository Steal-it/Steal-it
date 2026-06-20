using System;
using System.Collections.Generic;
using Ubiq.Rooms;
using UnityEngine;

public class RoomsListPanel : MonoBehaviour {
    public event EventHandler OnRoomJoined;

    [SerializeField]
    private GameObject listElementTemplate;

    private RoomClient roomClient;
    private List<IRoom> lastRoomList;
    private List<RoomsListElement> elementList = new List<RoomsListElement>();
    private float roomRefreshInterval = 1;
    private float roomRefreshTime;
    private Transform listContentTransform;

    void Start() {
        roomClient = NetworkReferenceManager.Instance.RoomClient;
        listContentTransform = listElementTemplate.transform.parent;

        roomClient.OnRooms.AddListener(OnRoomsDiscovered);
        roomClient.OnJoinedRoom.AddListener(OnJoinedRoom);
        // PlayerNotifications.OnNotification += UpdateConnectionStatus;
        UpdateAvailableRooms();

        listElementTemplate.SetActive(false);
    }

    void Update() {
        roomRefreshTime += Time.deltaTime;
        if (roomRefreshTime >= roomRefreshInterval) {
            roomClient.DiscoverRooms();
            roomRefreshTime = 0;
        }
    }

    private void OnRoomsDiscovered(List<IRoom> _roomList, RoomsDiscoveredRequest _request) {
        // Ignore filtered requests
        if (string.IsNullOrEmpty(_request.joincode)) {
            lastRoomList = _roomList;
            UpdateAvailableRooms();
        }
    }

    private void OnJoinedRoom(IRoom _arg) {
        UpdateAvailableRooms();

        // Immediately ask for a refresh (maybe the room we just left is now empty)
        roomClient.DiscoverRooms();
    }

    private void UpdateAvailableRooms() {
        List<IRoom> rooms = lastRoomList;
        if (rooms == null || rooms.Count == 0) {
            foreach (RoomsListElement element in elementList) {
                Destroy(element.gameObject);
            }
            elementList.Clear();

            return;
        }

        int elementIdx = 0;
        for (int roomIdx = 0; roomIdx < rooms.Count; elementIdx++, roomIdx++) {
            bool isRoomTheCurrentRoom = rooms[roomIdx].Name == roomClient.Room.Name;

            if (elementList.Count <= elementIdx) {
                elementList.Add(InstantiateElement(isRoomTheCurrentRoom));
            }

            elementList[elementIdx].Bind(roomClient, rooms[roomIdx]);

            // if (isRoomTheCurrentRoom) {
            //     connectionStatusText.text = "Connected";
            // }
        }

        while (elementList.Count > elementIdx) {
            Destroy(elementList[elementIdx].gameObject);
            elementList.RemoveAt(elementIdx);
        }
    }

    private RoomsListElement InstantiateElement(bool _isCurrentRoom) {
        GameObject elementGameObject = Instantiate(listElementTemplate, listContentTransform);
        // Hide the element list if it corresponds to the current room
        elementGameObject.SetActive(!_isCurrentRoom);

        RoomsListElement roomsListElement = elementGameObject.GetComponent<RoomsListElement>();
        roomsListElement.OnRoomJoined += (_sender, _event) => OnRoomJoined?.Invoke(this, EventArgs.Empty);

        return roomsListElement;
    }

    void OnDestroy() {
        if (roomClient != null) {
            roomClient.OnRooms.RemoveListener(OnRoomsDiscovered);
            roomClient.OnJoinedRoom.RemoveListener(OnJoinedRoom);
        }
    }
}
