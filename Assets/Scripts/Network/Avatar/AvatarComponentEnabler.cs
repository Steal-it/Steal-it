using UnityEngine;

[RequireComponent(typeof(NetworkObjectEnabler))]
public abstract class AvatarComponentEnabler : LocalAvatar {
    protected NetworkObjectEnabler NetworkObjectEnabler { get; private set; }
    [SerializeField]
    private GameObject[] gameObjects;
    protected GameObject[] GameObjects => gameObjects;

    protected override void Awake() {
        base.Awake();
        NetworkObjectEnabler = GetComponent<NetworkObjectEnabler>();
    }
}
