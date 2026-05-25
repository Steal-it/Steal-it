using System;
using Ubiq.Rooms;
using UnityEngine;
using TMPro;
using UnityEngine.Events;


public class BrowseMenuControl : MonoBehaviour
{
    public TextMeshProUGUI Name;

    [Serializable]
    public class BindEvent : UnityEvent<RoomClient, IRoom> { };
    public BindEvent OnBind;
    public void Bind(RoomClient client, IRoom room)
    {
        Name.text = room.Name;
        OnBind.Invoke(client,room);
    }
}
