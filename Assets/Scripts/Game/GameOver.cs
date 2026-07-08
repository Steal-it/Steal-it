using System.Collections;
using UnityEngine;

public class GameOver : MonoBehaviour {
    public static GameOver Instance { get; private set; }

    [SerializeField]
    private GameOverUI gameOverUI;

    void Awake() {
        if (Instance != null) {
            Debug.LogError($"An instance of {nameof(GameOver)} already exists!");
            return;
        }

        Instance = this;
    }

    private IEnumerator WaitForQuit(bool _isWinner) {
        yield return new WaitForSeconds(gameOverUI.ActivatePanel(_isWinner));

        Application.Quit();
    }

    public void Winner() {
        StartCoroutine(WaitForQuit(true));
    }

    public void Loser() {
        StartCoroutine(WaitForQuit(false));
    }
}
