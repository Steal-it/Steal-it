using System.Collections.Generic;
using System.Linq;
using Ubiq.Rooms;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MapConfigurationManager : MonoBehaviour {
    [SerializeField]
    private Transform playersSpawnPoint;
    [SerializeField]
    private Monster monster;
    [SerializeField]
    private Transform locks;

    private List<MapConfiguration> mapConfigurationArray;
    private List<Transform> keyList;

    void Start() {
        mapConfigurationArray = new List<MapConfiguration>();
        foreach (Transform child in transform) {
            if (child.gameObject.activeInHierarchy) {
                mapConfigurationArray.Add(child.GetComponent<MapConfiguration>());
            }
        }

        keyList = new List<Transform>();
        foreach (Transform child in locks) {
            foreach (XRGrabInteractable key in child.GetComponent<SocketFilter>().GrabInteractableList) {
                keyList.Add(key.transform);
            }
        }
    }

    private string GetAgreedUuid() {
        List<IPeer> peerList = new List<IPeer>(NetworkReferenceManager.Instance.RoomClient.Peers) {
            NetworkReferenceManager.Instance.RoomClient.Me
        };

        return peerList.OrderBy(peer => peer.uuid).First().uuid;
    }

    private int UuidToSeed(string uuid) {
        // Manually compute the hash of the UUID, same value from same string is not guaranteed by GetHashCode() .NET function
        unchecked {
            int hash = 17;
            foreach (char c in uuid)
                hash = hash * 31 + c;
            return hash;
        }
    }

    public void ApplyRandomConfiguration() {
        string agreedUuid = GetAgreedUuid();
        int seed = UuidToSeed(agreedUuid);

        System.Random random = new System.Random(seed);
        int mapConfigurationIndex = random.Next() % mapConfigurationArray.Count;
        mapConfigurationArray[mapConfigurationIndex].Apply(
            playersSpawnPoint,
            monster.transform,
            keyList.ToArray()
        );
    }
}
