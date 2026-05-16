using UnityEngine;
using System;

public class NewRoomButton : MonoBehaviour
{
    public Menu mainMenu;
    // Expected to be called by a UI element
    public void NewRoom ()
    {
        string roomName = Guid.NewGuid().ToString().Substring(0,7);
        mainMenu.roomClient.Join(
            name: roomName,
            publish: true);
    }
}