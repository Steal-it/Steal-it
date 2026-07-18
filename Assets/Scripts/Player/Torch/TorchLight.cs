using UnityEngine;

public class TorchLight : MonoBehaviour {
    public float MaxLightDistance => maxLightDistance;
    public float LightAngle => lightAngle;

    [SerializeField]
    private Transform lightEmitPointTransform;
    [SerializeField, Range(0, 120)]
    private float lightAngle = 10;
    [SerializeField, Range(3, 30)]
    private float maxLightDistance = 5;
    [SerializeField]
    private LayerMask wallLayer;

    private bool power;
    private MonsterLightDetector monster;
    private float coneRadius;

    void Update() {
        if (!power) {
            // If the torch is off and the player was flashing the monster ...
            if (monster != null) {
                // ... stop the monster light exposure
                monster.StopLightExposureTimer();
            }

            // The monster is not flashed anymore by the player
            monster = null;

            return;
        }

        // Check every collision with the light
        coneRadius = Mathf.Tan(lightAngle * Mathf.Deg2Rad * 0.5f) * maxLightDistance;
        RaycastHit[] hitArray = Physics.SphereCastAll(lightEmitPointTransform.position, coneRadius, lightEmitPointTransform.forward, maxLightDistance - coneRadius);
        bool isMonsterHit = false;
        foreach (RaycastHit hit in hitArray) {
            if (hit.transform.TryGetComponent(out MonsterLightDetector _monster)) {
                // Check if the monster is not behind a wall
                if (!Physics.Raycast(lightEmitPointTransform.position, lightEmitPointTransform.forward, maxLightDistance, wallLayer)) {
                    isMonsterHit = true;

                    // The first time the player illuminates the monster ...
                    if (monster == null) {
                        // ... start its light exposure time ...
                        monster = _monster;
                        monster.StartLightExposureTimer();
                    }
                }
            }
        }

        // ... otherwise, stop it if the monster was flashed (the torch is on but it is not illuminating the monster anymore)
        if (!isMonsterHit) {
            if (monster != null) {
                monster.StopLightExposureTimer();
                monster = null;
            }
        }
    }

    public void ToggleLight(object _, Torch.OnTorchTurnedEventArgs _eventArgs) {
        power = _eventArgs.IsTurnedOn;
    }

    void OnDrawGizmos() {
        if (!power) return;
        var transformData = lightEmitPointTransform;
        var gizmoStart = transformData.position;
        var gizmoEnd = gizmoStart + (transformData.forward * (maxLightDistance - coneRadius));
        var gizmoUp = transformData.up * coneRadius;
        var gizmoSide = transformData.right * coneRadius;
        Gizmos.DrawLine(gizmoStart, gizmoEnd);
        Gizmos.DrawLine(gizmoStart, gizmoEnd + gizmoSide);
        Gizmos.DrawLine(gizmoStart, gizmoEnd - gizmoSide);
        Gizmos.DrawLine(gizmoStart, gizmoEnd + gizmoUp);
        Gizmos.DrawLine(gizmoStart, gizmoEnd - gizmoUp);
        Gizmos.DrawWireSphere(gizmoEnd, coneRadius);
    }
}
