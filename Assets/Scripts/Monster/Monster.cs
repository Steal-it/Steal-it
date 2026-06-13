using System;
using UnityEngine;

public class Monster : NetworkObject {
    [SerializeField]
    private MonsterAI monsterAI;
    [SerializeField]
    private MonsterPlaceholder monsterPlaceholder;

    void Awake() {
        OnAwake(this);
    }

    void Start() {
        NetworkReferenceManager.Instance.MainMenu.OnNewRoomCreated += MainMenu_OnNewRoomCreated;
        NetworkReferenceManager.Instance.MainMenu.OnRoomJoined += MainMenu_OnRoomJoined;

        monsterAI.gameObject.SetActive(false);
        monsterPlaceholder.gameObject.SetActive(false);
    }

    void FixedUpdate() {
        OnFixedUpdate();
    }

    private void MainMenu_OnNewRoomCreated(object _sender, EventArgs _event) {
        EnableServerMonster();
    }

    private void MainMenu_OnRoomJoined(object _sender, EventArgs _event) {
        EnableClientMonster();
    }

    /// <summary>
    /// Enables the monster with its logic, used for the client that created the room, acting as a server.
    /// </summary>
    private void EnableServerMonster() {
        monsterAI.gameObject.SetActive(true);
        monsterPlaceholder.gameObject.SetActive(false);

        SelectObject(monsterAI.transform);
    }

    /// <summary>
    /// Disable the monster and its logic to use the placeholder, used for the clients that did not created the room.
    /// </summary>
    private void EnableClientMonster() {
        monsterAI.gameObject.SetActive(false);
        monsterPlaceholder.gameObject.SetActive(true);

        DeselectObject();
    }

    void OnDestroy() {
        NetworkReferenceManager.Instance.MainMenu.OnNewRoomCreated -= MainMenu_OnNewRoomCreated;
        NetworkReferenceManager.Instance.MainMenu.OnRoomJoined -= MainMenu_OnRoomJoined;
    }

    protected override void SendOnFixedUpdate() { }

    protected override void NotSendOnFixedUpdate() { }

    protected override void OwnedOnReceived() { }

    protected override void NotOwnedOnReceived() { }
}
