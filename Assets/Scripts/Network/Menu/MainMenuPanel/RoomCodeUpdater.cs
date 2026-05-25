using Ubiq.Rooms;
using UnityEngine;
using TMPro;

public class RoomCodeUpdater : MonoBehaviour
{
    [SerializeField]
    private Menu mainMenu;

    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        mainMenu.RoomClient.OnJoinedRoom.AddListener(OnJoinRoomListener);
    }

    private void OnJoinRoomListener(IRoom room)
    {
        if (room != null && room.UUID != null && room.UUID.Length > 0)
        {
            text.text=$"Room name: {room.Name}";
        }
    }

    void OnDestroy()
    {
        mainMenu.RoomClient.OnJoinedRoom.RemoveListener(OnJoinRoomListener);
    }

}
