using UnityEngine;

public class Monster : MonoBehaviour {
    [SerializeField]
    private MonsterStateManager monsterStateManager;

    private float lightExposureTimer;
    private int lightExposureCounter;

    void Update() {
        if (monsterStateManager.CurrentStateKey == MonsterStateManager.StateKey.Stunned) return;

        // If at least one player is flashing the monster, start light exposure timer
        if (lightExposureCounter > 0) {
            lightExposureTimer -= Time.deltaTime;

            if (lightExposureTimer <= 0) {
                monsterStateManager.ChangeState(MonsterStateManager.StateKey.Stunned);
            }
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

    void OnDrawGizmosSelected() {
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
    }
}
