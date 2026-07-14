using Ubiq.Messaging;
using UnityEngine;

public class MonsterLightDetector : NetworkComponent {
    [SerializeField]
    private MonsterStateManager monsterStateManager;  // TODO: remove, used only for gizmos
    [SerializeField, Range(0.2f, 1)]
    private float lightExposureTime = 0.5f;

    private LevelManager levelManager;
    private float lightExposureTimer;
    private int lightExposureCounter;

    void Start() {
        RegisterContext(this);

        levelManager = NetworkReferenceManager.Instance.LevelManager;
    }

    void Update() {
        // Allow only the client that acts as the server to run the logic
        if (!levelManager.IsClientAsServer) return;

        // The monster can be flashed only when it chases a player
        if (monsterStateManager.CurrentStateKey != MonsterStateManager.StateKey.Chase) return;

        // If at least one player is flashing the monster, start light exposure timer
        if (lightExposureCounter > 0) {
            lightExposureTimer -= Time.deltaTime;

            if (lightExposureTimer <= 0) {
                monsterStateManager.ChangeState(MonsterStateManager.StateKey.Flashed);
            }
        }
    }

    public void StartLightExposureTimer() {
        if (!levelManager.IsClientAsServer) {
            // If a client started illuminating the monster, notify to the server
            StartMonsterLightExposure message = new StartMonsterLightExposure();
            Context.SendJson(message);

            return;
        }

        if (lightExposureCounter == 0) {
            // Start the light exposure timer only at the first flash
            lightExposureTimer = lightExposureTime;
        }

        lightExposureCounter++;
    }

    public void StopLightExposureTimer() {
        if (!levelManager.IsClientAsServer) {
            // If a client stopped illuminating the monster, notify to the server
            StopMonsterLightExposure message = new StopMonsterLightExposure();
            Context.SendJson(message);

            return;
        }

        if (lightExposureCounter == 0) return;

        lightExposureCounter--;
    }

    public override void ProcessMessage(ReferenceCountedSceneGraphMessage _message) {
        BaseMessage message = _message.FromJson<BaseMessage>();
        switch (message.type) {
            case StartMonsterLightExposure.TYPE: {
                    if (levelManager.IsClientAsServer) {
                        StartLightExposureTimer();
                    }
                }
                break;
            case StopMonsterLightExposure.TYPE: {
                    if (levelManager.IsClientAsServer) {
                        StopLightExposureTimer();
                    }
                }
                break;
            default:
                Debug.LogWarning("Received unknown message! " + message.type);
                break;
        }
    }

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
