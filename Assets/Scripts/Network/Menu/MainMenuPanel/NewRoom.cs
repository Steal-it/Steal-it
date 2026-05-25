using UnityEngine;
using System;
using UnityEngine.UI;

public class NewRoomButton : MonoBehaviour
{
    [SerializeField]
    private Menu mainMenu;
    // Expected to be called by a UI element
    [SerializeField]
    private Button button;

    private void Awake() {
        button.onClick.AddListener(NewRoom);
    }

    public void NewRoom ()
    {
        string roomName = Guid.NewGuid().ToString().Substring(0,7);
        mainMenu.RoomClient.Join(
            name: roomName,
            publish: true);
    }

    private void OnDestroy() {
        button.onClick.RemoveListener(NewRoom);
    }
}