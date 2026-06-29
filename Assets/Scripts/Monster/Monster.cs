using UnityEngine;

public class Monster : MonoBehaviour {
    [SerializeField]
    private NetworkMovement networkMovement;
    [SerializeField]
    private NetworkAnimation networkAnimation;
    [SerializeField]
    private MonsterAI monsterAI;
    [SerializeField]
    private Transform monsterPlaceholder;
    [SerializeField]
    private Transform commonTransform;
    [SerializeField]
    private MonsterAnimator monsterAnimator;

    void Start() {
        NetworkReferenceManager.Instance.LevelManager.OnGameLoaded += LevelManager_OnGameLoaded;
        monsterAnimator.OnAnimationChanged += MonsterAnimator_OnAnimationChanged;
        networkAnimation.OnMessageReceived += NetworkAnimation_OnMessageReceived;

        monsterAI.gameObject.SetActive(false);
        monsterPlaceholder.gameObject.SetActive(false);
        commonTransform.gameObject.SetActive(false);
    }

    void FixedUpdate() {
        if (networkMovement.Transform != null) {
            commonTransform.SetPositionAndRotation(networkMovement.Transform.position, networkMovement.Transform.rotation);
        }
    }

    private void LevelManager_OnGameLoaded(object _sender, LevelManager.OnGameLoadedEventArgs _event) {
        if (_event.IsClientAsServer) {
            EnableServerMonster();
        } else {
            EnableClientMonster();
        }
        commonTransform.gameObject.SetActive(true);
    }

    private void MonsterAnimator_OnAnimationChanged(object _sender, AbstractNetworkAnimator.OnAnimationChangedEventArgs _event) {
        networkAnimation.SendAnimationParameters(_event.ParameterDictionary);
    }

    private void NetworkAnimation_OnMessageReceived(object _sender, NetworkAnimation.OnMessageReceivedEventArgs _event) {
        monsterAnimator.SetParameterDictionary(_event.ParameterDictionary);
    }

    /// <summary>
    /// Enables the monster with its logic, used for the client that created the room, acting as a server.
    /// </summary>
    private void EnableServerMonster() {
        monsterAI.gameObject.SetActive(true);
        monsterPlaceholder.gameObject.SetActive(false);

        networkMovement.Transform = monsterAI.transform;
        networkMovement.SelectObject();
    }

    /// <summary>
    /// Disable the monster and its logic to use the placeholder, used for the clients that did not created the room.
    /// </summary>
    private void EnableClientMonster() {
        monsterAI.gameObject.SetActive(false);
        monsterPlaceholder.gameObject.SetActive(true);

        // TODO: use common only
        networkMovement.Transform = monsterPlaceholder.transform;
    }

    void OnDestroy() {
        NetworkReferenceManager.Instance.LevelManager.OnGameLoaded -= LevelManager_OnGameLoaded;
        monsterAnimator.OnAnimationChanged -= MonsterAnimator_OnAnimationChanged;
        networkAnimation.OnMessageReceived -= NetworkAnimation_OnMessageReceived;
    }
}
