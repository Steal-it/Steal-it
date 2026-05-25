using System;
using Ubiq.Rooms;
using UnityEngine;
using TMPro;
using UnityEngine.Events;


public class BrowseMenuControl : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI roomNameText;

    [Serializable]
    public class BindEvent : UnityEvent<RoomClient, IRoom> { };
    public BindEvent OnBind;
    public void Bind(RoomClient _client, IRoom _room)
    {
        roomNameText.text = _room.Name;
        OnBind.Invoke(_client,_room);
    }
}
