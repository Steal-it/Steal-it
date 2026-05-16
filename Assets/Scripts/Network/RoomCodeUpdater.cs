using Ubiq.Rooms;
using UnityEngine;
using TMPro;

public class RoomCodeUpdater : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private RoomClient rc;

    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        rc.OnJoinedRoom.AddListener(OnJoinRoomListener);
    }

    private void OnJoinRoomListener(IRoom room)
    {
        if (room != null && room.UUID != null && room.UUID.Length > 0)
        {
            text.text=$"Room name: {room.Name}";
        }
    }
}
