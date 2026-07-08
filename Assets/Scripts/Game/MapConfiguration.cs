using UnityEngine;

public class MapConfiguration : MonoBehaviour {
    [SerializeField]
    private Transform playersSpawnPoint;
    [SerializeField]
    private Transform monsterSpawnPoint;
    [SerializeField]
    private Transform keys;

    private Transform[] keySpawnPointArray;

    void Start() {
        keySpawnPointArray = new Transform[keys.childCount];
        for (int i = 0; i < keySpawnPointArray.Length; i++) {
            keySpawnPointArray[i] = keys.GetChild(i);
        }
    }

    private void SetPlayersSpawnPoint(Transform _playersSpawnPoint) {
        _playersSpawnPoint.SetPositionAndRotation(playersSpawnPoint.position, playersSpawnPoint.rotation);
    }

    private void SetMonsterSpawnPoint(Transform _monster) {
        _monster.SetPositionAndRotation(monsterSpawnPoint.position, monsterSpawnPoint.rotation);
    }

    private void SetKeys(Transform _keys) {
        if (_keys.childCount != keySpawnPointArray.Length) {
            Debug.LogWarning("Mismatching size of keys and spawn points!");
            return;
        }

        for (int i = 0; i < keySpawnPointArray.Length; i++) {
            _keys.GetChild(i).SetPositionAndRotation(keySpawnPointArray[i].position, keySpawnPointArray[i].rotation);
        }
    }

    public void Apply(Transform _playersSpawnPoint, Transform _monster, Transform _keys) {
        SetPlayersSpawnPoint(_playersSpawnPoint);
        SetMonsterSpawnPoint(_monster);
        SetKeys(_keys);
    }
}
