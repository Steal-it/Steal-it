public class MonsterPlaceholder : NetworkObject {
    void Awake() {
        OnAwake();
    }

    protected override void SendOnFixedUpdate() { }

    protected override void NotSendOnFixedUpdate() { }

    protected override void OwnedOnReceived() { }

    protected override void NotOwnedOnReceived() { }
}
