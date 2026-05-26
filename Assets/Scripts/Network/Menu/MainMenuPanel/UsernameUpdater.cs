using UnityEngine;
using Ubiq.Rooms;
using Ubiq;
using TMPro;

public class UsernameUpdater : MonoBehaviour
{
    [SerializeField]
    private Menu mainMenu;

    private TextMeshProUGUI text;
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        mainMenu.RoomClient.OnPeerUpdated.AddListener(OnPeerListener);
    }

    private void OnPeerListener(IPeer peer)
    {
        if(mainMenu.RoomClient.Me == peer)
        {
            updateUsername();
        }    
    }

    private void updateUsername()
    {
        if(mainMenu.RoomClient.Me != null)
        {
            string name=mainMenu.RoomClient.Me[DisplayNameManager.KEY];
            if(!string.IsNullOrEmpty(name))
            {
                text.text = $"Welcome {name}";
            }
        }
    }

    void OnDestroy()
    {
        mainMenu.RoomClient.OnPeerUpdated.RemoveListener(OnPeerListener);
    }
}
