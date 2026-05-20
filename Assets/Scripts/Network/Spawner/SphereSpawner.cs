using System.Collections;
using Ubiq.Spawning;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SphereSpawner : MonoBehaviour
{
    [Header("Prefab to spawn")]
    public GameObject sphereTemplate;

    private NetworkSpawnManager spawnManager;
    private XRInteractionManager interactionManager;
    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        if (!interactable)
        {
            Debug.LogError("No XRSimpleInteractable found on SphereSpawner object");
        }

        interactionManager = FindObjectOfType<XRInteractionManager>();
        if (!interactionManager)
        {
            Debug.LogError("No XRInteractionManager found in scene");
        }

        // Find network spawn manager
        spawnManager = NetworkSpawnManager.Find(this);
        if (!spawnManager)
        {
            Debug.LogError("No NetworkSpawnManager found in scene");
        }
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

    private void OnSelectEntered(SelectEnterEventArgs eventArgs)
    {
        if (!spawnManager || !interactionManager)
        {
            return;
        }

        
        var go = spawnManager.SpawnWithPeerScope(sphereTemplate);
        go.SetActive(true);
        var spawnedSphere = go.GetComponent<SphereManager>();

        if (spawnedSphere == null)
        {
            Debug.LogError("Spawned object does not have a SphereManager component");
            return;
        }

        spawnedSphere.transform.position = transform.position;
        spawnedSphere.owner = true;

        var xrGrab = go.GetComponent<XRGrabInteractable>();
        if (xrGrab == null)
        {
            Debug.LogError("Spawned prefab is missing XRGrabInteractable");
            return;
        }

        xrGrab.interactionManager = interactionManager;
        xrGrab.enabled = true;

        var interactor = eventArgs.interactorObject;
        if (interactor != null)
        {
            interactionManager.SelectExit(interactor, interactable);

            StartCoroutine(SelectNextFrame(interactor, xrGrab));
        }
    }

    private IEnumerator SelectNextFrame(IXRSelectInteractor interactor, XRGrabInteractable interactable)
    {
        yield return null;

        if (interactionManager != null)
        {
            interactionManager.SelectEnter(interactor, interactable);
        }
    }
}