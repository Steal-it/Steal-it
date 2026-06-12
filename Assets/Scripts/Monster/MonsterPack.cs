using System;
using UnityEngine;

public class MonsterPack : MonoBehaviour {
    [SerializeField]
    private MonsterStateManager monsterStateManager;
    [SerializeField]
    private Monster monster;
    [SerializeField]
    private GameObject monsterPlaceholder;

    void Start() {
        NetworkReferenceManager.Instance.NewRoomButton.OnNewRoomCreated += NewRoomButton_OnNewRoomCreated;
    }

    private void NewRoomButton_OnNewRoomCreated(object _sender, EventArgs _event) {
        EnableServerMonster();
    }

    private void BrowseMenuControlJoinButton_OnClientJoined(object _sender, EventArgs _event) {
        EnableClientMonster();
    }

    /// <summary>
    /// Enables the monster with its logic, used for the client that created the room, acting as a server.
    /// </summary>
    private void EnableServerMonster() {
        monsterStateManager.gameObject.SetActive(true);
        monster.gameObject.SetActive(true);
        monsterPlaceholder.SetActive(false);
    }

    /// <summary>
    /// Disable the monster and its logic to use the placeholder, used for the clients that did not created the room.
    /// </summary>
    private void EnableClientMonster() {
        monsterStateManager.gameObject.SetActive(false);
        monster.gameObject.SetActive(false);
        monsterPlaceholder.SetActive(true);
    }
}
