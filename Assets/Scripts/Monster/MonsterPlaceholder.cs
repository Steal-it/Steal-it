public class MonsterPlaceholder : AbstractNetworkObject {
    void Awake() {
        OnAwake(this);
    }

    protected override void SendOnFixedUpdate() { }

    protected override void NotSendOnFixedUpdate() { }

    protected override void OwnedOnReceived() { }

    protected override void NotOwnedOnReceived() { }
}
