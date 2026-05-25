using UnityEngine;

public class MonsterSpawnPoint : MonoBehaviour {
    [SerializeField]
    private GameObject monsterPrefab;

    public AIMonster Spawn(Vector3 _direction) {
        GameObject monsterGameObject = Instantiate(monsterPrefab, transform.position, Quaternion.Euler(_direction));
        return monsterGameObject.GetComponent<AIMonster>();
    }
}
