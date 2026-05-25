using System.Collections;
using Ubiq.Rooms;
using Ubiq.Spawning;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SphereSpawner : MonoBehaviour
{
    [Header("Prefab to spawn")]
    public GameObject sphereTemplate;

    [SerializeField]
    private RoomClient roomClient;

    public NetworkSpawnManager spawnManager;
    private XRInteractionManager interactionManager;
    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        interactionManager = FindFirstObjectByType<XRInteractionManager>();
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnSelectEntered);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEntered);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs _eventArgs)
    {
        var go = spawnManager.SpawnWithPeerScope(sphereTemplate);
        go.SetActive(true);
        var spawnedSphere = go.GetComponent<SphereManager>();

        spawnedSphere.transform.position = transform.position;
        spawnedSphere.OriginalSender = roomClient.Me.uuid;

        var xrGrab = go.GetComponent<XRGrabInteractable>();

        xrGrab.interactionManager = interactionManager;
        xrGrab.enabled = true;

        var interactor = _eventArgs.interactorObject;
        interactionManager.SelectEnter(interactor, interactable);
        interactionManager.SelectExit(interactor, interactable);
    }

}