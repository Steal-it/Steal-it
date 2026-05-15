using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ubiq.Spawning;

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class Spawner : MonoBehaviour {
    [SerializeField]
    private Spawnable spawnable;

    private NetworkSpawnManager spawnManager;
    private XRSimpleInteractable interactable;
    private XRInteractionManager interactionManager;

    private void Start() {
        spawnManager = NetworkSpawnManager.Find(this);

        interactable = GetComponent<XRSimpleInteractable>();
        interactionManager = interactable.interactionManager;

        interactable.selectEntered.AddListener(XRGrabInteractable_SelectEntered);
    }

    private void OnDestroy() {
        interactable.selectEntered.RemoveListener(XRGrabInteractable_SelectEntered);
    }

    private void XRGrabInteractable_SelectEntered(SelectEnterEventArgs eventArgs) {
        // GameObject go = spawnManager.SpawnWithPeerScope(spawnable.gameObject);
        // Spawnable n_object = go.GetComponent<Spawnable>();
        // n_object.transform.position = transform.position;
        // n_object.owner = true;

        GameObject n_object = Instantiate(spawnable.gameObject, transform.position, Quaternion.identity);

        if (!interactionManager) {
            return;
        }

        // Force the interactor(hand) to stop selecting the box and select the firework
        var selectInteractor = eventArgs.interactorObject;
        if (selectInteractor != null) {
            interactionManager.SelectExit(
                selectInteractor,
                this.interactable);
            interactionManager.SelectEnter(
                selectInteractor,
                n_object.GetComponent<XRGrabInteractable>());
        }
    }
}