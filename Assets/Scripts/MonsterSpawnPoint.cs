using UnityEngine;

public class MonsterSpawnPoint : MonoBehaviour {
    [SerializeField]
    private GameObject monsterPrefab;

    public Monster Spawn(Transform _playerTransform) {
        GameObject monsterGameObject = Instantiate(monsterPrefab, transform.position, Quaternion.identity);
        monsterGameObject.transform.LookAt(_playerTransform);

        Monster monster = monsterGameObject.GetComponent<Monster>();
        monster.Initialize(_playerTransform);

        return monster;
    }
}
