using UnityEngine;
using Ubiq.Rooms;
using Ubiq;
using TMPro;

public class UsernameUpdater : MonoBehaviour
{
    [SerializeField]
    private RoomClient rc;

    private TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        rc.OnPeerUpdated.AddListener(OnPeerListener);
    }

    private void OnPeerListener(IPeer peer)
    {
        if(rc.Me == peer)
        {
            updateUsername();
        }    
    }

    private void updateUsername()
    {
        if(rc.Me != null)
        {
            string name=rc.Me[DisplayNameManager.KEY];
            if(!string.IsNullOrEmpty(name))
            {
                text.text = $"Welcome {name}";
            }
        }
    }
}
