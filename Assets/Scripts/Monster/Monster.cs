using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour {
    [SerializeField]
    private NetworkMovement networkMovement;
    [SerializeField]
    private NetworkAnimation networkAnimation;
    [SerializeField]
    private NetworkAudio networkAudio;
    [SerializeField]
    private NavMeshAgent monsterAgent;
    [SerializeField]
    private Transform commonTransform;
    [SerializeField]
    private MonsterAnimator monsterAnimator;
    [SerializeField]
    private MonsterSFXManager monsterSFXManager;

    void Start() {
        NetworkReferenceManager.Instance.LevelManager.OnGameLoaded += LevelManager_OnGameLoaded;
        monsterAnimator.OnAnimationChanged += MonsterAnimator_OnAnimationChanged;
        networkAnimation.OnMessageReceived += NetworkAnimation_OnMessageReceived;
        monsterSFXManager.OnSFXChanged += MonsterSFXManager_OnSFXChanged;
        networkAudio.OnMessageReceived += NetworkAudio_OnMessageReceived;

        monsterAgent.gameObject.SetActive(false);
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

    private void MonsterAnimator_OnAnimationChanged(object _sender, AnimatorNetworkExtension.OnAnimationChangedEventArgs _event) {
        networkAnimation.SendAnimationParameters(_event.ParameterDictionary);
    }

    private void NetworkAnimation_OnMessageReceived(object _sender, NetworkAnimation.OnMessageReceivedEventArgs _event) {
        monsterAnimator.SetParameterDictionary(_event.ParameterDictionary);
    }

    private void MonsterSFXManager_OnSFXChanged(object _sender, SFXManagerNetworkExtension.OnSFXChangedEventArgs _event) {
        networkAudio.SendSFXs(_event.SFXDictionary);
    }

    private void NetworkAudio_OnMessageReceived(object _sender, NetworkAudio.OnMessageReceivedEventArgs _event) {
        monsterSFXManager.SetSFXDictionary(_event.SFXDictionary);
    }

    /// <summary>
    /// Enables the monster with its logic, used for the client that created the room, acting as a server.
    /// </summary>
    private void EnableServerMonster() {
        monsterAgent.gameObject.SetActive(true);

        networkMovement.Transform = monsterAgent.transform;
        networkMovement.SelectObject();
    }

    /// <summary>
    /// Disable the monster and its logic to use the placeholder, used for the clients that did not created the room.
    /// </summary>
    private void EnableClientMonster() {
        monsterAgent.gameObject.SetActive(false);

        networkMovement.Transform = commonTransform;
    }

    void OnDestroy() {
        NetworkReferenceManager.Instance.LevelManager.OnGameLoaded -= LevelManager_OnGameLoaded;
        monsterAnimator.OnAnimationChanged -= MonsterAnimator_OnAnimationChanged;
        networkAnimation.OnMessageReceived -= NetworkAnimation_OnMessageReceived;
        monsterSFXManager.OnSFXChanged -= MonsterSFXManager_OnSFXChanged;
    }
}
