using Ubiq.Geometry;
using Ubiq.Messaging;
using UnityEngine;

public class Monster : MonoBehaviour {
    protected NetworkContext Context { private get; set; }
    protected bool AmIOwner { private get; set; }
    protected bool AmISender { private get; set; }



    [SerializeField]
    private MonsterStateManager monsterStateManager;

    private float lightExposureTimer;
    private int lightExposureCounter;

    void Awake() {
        AmIOwner = false;

        Context = NetworkScene.Register(this);
    }

    // void OnEnable() {
    //     SelectObject();
    // }

    void Update() {
        if (monsterStateManager.CurrentStateKey == MonsterStateManager.StateKey.Stunned) return;

        // If at least one player is flashing the monster, start light exposure timer
        if (lightExposureCounter > 0) {
            lightExposureTimer -= Time.deltaTime;
            print(lightExposureTimer);

            if (lightExposureTimer <= 0) {
                monsterStateManager.ChangeState(MonsterStateManager.StateKey.Stunned);
            }
        }
    }

    void FixedUpdate() {
        // Only if I am the sender I transmit the position
        if (true) {
            SendMessage();
        } else {
        }
    }

    protected void SelectObject() {
        AmIOwner = true;
        AmISender = true;
        SendMessage();
    }

    protected void ReleaseObject() {
        AmIOwner = false;
        SendMessage();
    }

    protected void DeselectObject() {
        AmIOwner = false;
        AmISender = false;
        SendMessage();
    }

    protected void SendMessage() {
        print("Send");
        MovementMessage message = new MovementMessage {
            Position = Transforms.ToLocal(transform, Context.Scene.transform),
            IsOwned = AmIOwner
        };

        Context.SendJson(message);
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        print("Received");
        MovementMessage message = _message.FromJson<MovementMessage>();
        Pose pose = Transforms.ToWorld(message.Position, Context.Scene.transform);
        transform.SetPositionAndRotation(pose.position, pose.rotation);

        if (message.IsOwned) {
            // If object is taken by another, the current player is no longer the amISender
            AmISender = false;
        } else {
        }
    }

    public void StartLightExposureTimer() {
        if (lightExposureCounter == 0) {
            // Start the light exposure timer only at the first flash
            lightExposureTimer = monsterStateManager.LightExposureTime;
        }

        lightExposureCounter++;
    }

    public void StopLightExposureTimer() {
        if (lightExposureCounter == 0) return;

        lightExposureCounter--;
    }

    // void OnDisable() {
    //     DeselectObject();
    // }

    void OnDrawGizmos() {
        if (monsterStateManager == null) return;

        int numLines = 6;
        float angleOffset = monsterStateManager.ViewAngle / numLines;
        for (int i = 0; i < numLines + 1; i++) {
            Vector3 line = DirFromAngle(-monsterStateManager.ViewAngle / 2 + angleOffset * i);

            Gizmos.DrawLine(transform.position, transform.position + line * monsterStateManager.ViewRadius);
        }

        Vector3 DirFromAngle(float _angleDegrees) {
            // Offset by the agent's current Y rotation so the cone rotates with it
            _angleDegrees += transform.eulerAngles.y;
            return new Vector3(Mathf.Sin(_angleDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(_angleDegrees * Mathf.Deg2Rad));
        }

        // Gizmos.DrawWireSphere(transform.position, monsterStateManager.StunnedMinDistanceDestination);
    }
}
