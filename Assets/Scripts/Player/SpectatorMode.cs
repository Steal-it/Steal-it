using System.Collections;
using UnityEngine;

public class SpectatorMode : MonoBehaviour {
    private bool enable;
    private string playerUUID;
    private AudioSource lostAudioSource;
    private TorsoIdentifier torso;

    void Start() {
        NetworkReferenceManager.Instance.SpectatorModeManager.OnSpectatorModeChanged += SpectatorModeManager_OnSpectatorModeChange;
        playerUUID = transform.parent.name;
        lostAudioSource = GetComponent<AudioSource>();

        if (playerUUID != "Local Avatar") {
            playerUUID = playerUUID.Split('#')[1];
        }

        torso = gameObject.transform.parent.gameObject.GetComponentInChildren<TorsoIdentifier>();
    }

    private void UpdateVisibility() {
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
            Collider col = torso.GetComponent<Collider>();
            col.enabled = false;
        }
    }

    private void Disable() {
        gameObject.transform.parent.gameObject.SetActive(true);
        if (torso && playerUUID != "Local Avatar") {
            Collider col = torso.GetComponent<Collider>();
            col.enabled = true;
        }
    }

    private void SpectatorModeManager_OnSpectatorModeChange(object _sender, SpectatorModeManager.OnSpectatorModeChangeEventArgs _event) {
        if (enable) return;

        // Case of an avatar being killed by the monster
        // The coroutine allow for the sound to play when the Local Avatar is dead, otherwise it will stop since the avatar is deactivated

        StartCoroutine(HandleSpectatorChange(_event.PlayerUUID));
    }

    private IEnumerator HandleSpectatorChange(string receivedPlayerUUID) {
        if (playerUUID == "Local Avatar" && !enable) {
            //If a player lost play the lost sound. Since this function can be called both when spectator mode is to be activated (enable = false -> true) and deactivated (enable = true -> false), the sound has to play only when the spectator mode is being activated (enable = false -> true)

            lostAudioSource.Play();

            yield return new WaitUntil(() => !lostAudioSource.isPlaying);
        }

        if (receivedPlayerUUID == playerUUID || (playerUUID == "Local Avatar" && receivedPlayerUUID == NetworkReferenceManager.Instance.RoomClient.Me.uuid)) {
            UpdateVisibility();
        }
    }

    void OnDestroy() {
        NetworkReferenceManager.Instance.SpectatorModeManager.OnSpectatorModeChanged -= SpectatorModeManager_OnSpectatorModeChange;
    }
}