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
        NetworkReferenceManager.Instance.LevelManager.OnGameLoaded += LevelManager_OnGameLoaded;

        monsterAI.gameObject.SetActive(false);
        monsterPlaceholder.gameObject.SetActive(false);
    }

    void FixedUpdate() {
        OnFixedUpdate();
    }

    private void LevelManager_OnGameLoaded(object _sender, LevelManager.OnGameLoadedEventArgs _event) {
        if (_event.IsClientAsServer) {
            EnableServerMonster();
        } else {
            EnableClientMonster();
        }
    }

    /// <summary>
    /// Enables the monster with its logic, used for the client that created the room, acting as a server.
    /// </summary>
    private void EnableServerMonster() {
        monsterAI.gameObject.SetActive(true);
        monsterPlaceholder.gameObject.SetActive(false);

        Transform = monsterAI.transform;
        SelectObject();
    }

    /// <summary>
    /// Disable the monster and its logic to use the placeholder, used for the clients that did not created the room.
    /// </summary>
    private void EnableClientMonster() {
        monsterAI.gameObject.SetActive(false);
        monsterPlaceholder.gameObject.SetActive(true);

        Transform = monsterPlaceholder.transform;
    }

    protected override void SendOnFixedUpdate() { }

    protected override void NotSendOnFixedUpdate() { }

    protected override void OwnedOnReceived() { }

    protected override void NotOwnedOnReceived() { }

    void OnDestroy() {
        NetworkReferenceManager.Instance.LevelManager.OnGameLoaded -= LevelManager_OnGameLoaded;
    }
}
