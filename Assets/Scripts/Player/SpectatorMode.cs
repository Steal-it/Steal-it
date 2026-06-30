using System.Collections;
using UnityEngine;

public class SpectatorMode : MonoBehaviour {

    private bool enable;

    private string playerUUID;

    private AudioSource lostAudioSource;

    private TorsoIdentifier torso;

    void Start() {
        NetworkReferenceManager.Instance.SpectatorModeManager.OnSpectatorModeChange += SpectatorModeManager_OnSpectatorModeChange;
        playerUUID = transform.parent.name;
        lostAudioSource = GetComponent<AudioSource>();

        if (playerUUID != "Local Avatar") {
            playerUUID = playerUUID.Split('#')[1];
        }

        torso = gameObject.transform.parent.gameObject.GetComponentInChildren<TorsoIdentifier>();
    }

    void updateVisibility() {
        enable = !enable;
        if (enable) {
            Enable();
        } else {
            Disable();
        }
    }

    private void Enable() {
        gameObject.transform.parent.gameObject.SetActive(false);

        //Disable the collider
        if (torso) {
            var col = torso.GetComponent<Collider>();
            col.enabled = false;
        }
    }

    private void Disable() {
        gameObject.transform.parent.gameObject.SetActive(true);
        if (torso && playerUUID != "Local Avatar") {
            var col = torso.GetComponent<Collider>();
            col.enabled = true;
        }
    }

    private void SpectatorModeManager_OnSpectatorModeChange(object _sender,
    SpectatorModeManager.OnSpectatorModeChangeEventArgs _args) {
        //Case of an avatar being killed by the monster
        //The coroutine allow for the sound to play when the Local Avatar is dead, otherwise it will stop since the avatar is deactivated

        StartCoroutine(HandleSpectatorChange(_args.PlayerUUID));
    }

    private IEnumerator HandleSpectatorChange(string receivedPlayerUUID) {
        if (playerUUID == "Local Avatar") {
            lostAudioSource.Play();

            yield return new WaitUntil(() => !lostAudioSource.isPlaying);
        }

        if (receivedPlayerUUID == playerUUID || (playerUUID == "Local Avatar" && receivedPlayerUUID == NetworkReferenceManager.Instance.RoomClient.Me.uuid)) {
            updateVisibility();
        }
    }

    void OnDestroy() {
        NetworkReferenceManager.Instance.SpectatorModeManager.OnSpectatorModeChange -= SpectatorModeManager_OnSpectatorModeChange;
    }
}