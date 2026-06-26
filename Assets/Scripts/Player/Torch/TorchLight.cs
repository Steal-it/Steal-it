using UnityEngine;

public class TorchLight : MonoBehaviour {
    [SerializeField]
    private Light torchLight;
    [SerializeField]
    private Transform lightEmitPointTransform;
    [SerializeField, Range(0.05f, 0.5f)]
    private float lightRadius = 0.3f;
    [SerializeField, Range(3, 10)]
    private float maxLightDistance = 5;
    private bool power;
    private MonsterAI monster;

    void OnValidate() {
        torchLight.range = maxLightDistance;
        torchLight.innerSpotAngle = lightRadius * 20;
        torchLight.spotAngle = lightRadius * 20;
    }

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
        int sphereCount = 5;
        float sphereOffset = maxLightDistance / sphereCount;
        for (int i = 1; i < sphereCount + 1; i++) {
            Gizmos.DrawWireSphere(lightEmitPointTransform.position + lightEmitPointTransform.forward * sphereOffset * i, lightRadius);
        }
    }

    public void ToggleLight(object _, Torch.OnTorchTurnedEventArgs _eventArgs) {
        torchLight.enabled = _eventArgs.isTurnedOn;
        power = torchLight.enabled;
    }

    public void InPocket(bool _isInPocket) {
        if (_isInPocket) {
            torchLight.enabled = false;
        } else {
            torchLight.enabled = power;
        }
    }

}
