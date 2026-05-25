using UnityEngine;
using Ubiq.Rooms;
using Ubiq;
using TMPro;

public class UsernameUpdater : MonoBehaviour
{
    [SerializeField]
    private Menu _mainMenu;

    private TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        _mainMenu.roomClient.OnPeerUpdated.AddListener(OnPeerListener);
    }

    private void OnPeerListener(IPeer peer)
    {
        if(_mainMenu.roomClient.Me == peer)
        {
            updateUsername();
        }    
    }

    private void updateUsername()
    {
        if(_mainMenu.roomClient.Me != null)
        {
            string name=_mainMenu.roomClient.Me[DisplayNameManager.KEY];
            if(!string.IsNullOrEmpty(name))
            {
                text.text = $"Welcome {name}";
            }
        }
    }
}
