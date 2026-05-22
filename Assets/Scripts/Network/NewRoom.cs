using UnityEngine;
using System;

public class NewRoomButton : MonoBehaviour
{
    [SerializeField]
    private Menu _mainMenu;
    // Expected to be called by a UI element
    public void NewRoom ()
    {
        string roomName = Guid.NewGuid().ToString().Substring(0,7);
        _mainMenu.roomClient.Join(
            name: roomName,
            publish: true);
    }
}