using UnityEngine;

public class TorchLight : MonoBehaviour {
    [SerializeField]
    private Transform lightEmitPointTransform;
    [SerializeField]
    private float lightRadius = 10;
    [SerializeField, Range(3, 10)]
    private float maxLightDistance = 5;
    private bool power;
    private MonsterAI monster;

    public float MaxLightDistance { get => maxLightDistance; }
    public float LightRadius { get => lightRadius; }

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
        RaycastHit[] hitArray = Physics.SphereCastAll(lightEmitPointTransform.position, lightRadius, lightEmitPointTransform.forward, maxLightDistance);
        bool isMonsterHit = false;
        foreach (RaycastHit hit in hitArray) {
            if (hit.transform.TryGetComponent(out MonsterAI _monster)) {
                isMonsterHit = true;

                // The first time the player illuminates the monster ...
                if (monster == null) {
                    // ... start its light exposure time ...
                    monster = _monster;
                    monster.StartLightExposureTimer();
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

    void OnDrawGizmos() {
        if (!power) return;
        var transformData = lightEmitPointTransform;
        var gizmoStart = transformData.position;
        var coneRadius = Mathf.Tan(lightRadius * Mathf.Deg2Rad * 0.5f) * maxLightDistance;
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

    public void ToggleLight(object _, Torch.OnTorchTurnedEventArgs _eventArgs) {
        power = _eventArgs.isTurnedOn;
    }


}
