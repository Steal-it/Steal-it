using System.Collections;
using UnityEngine;

public class GameOver : MonoBehaviour {
    public static GameOver Instance { get; private set; }

    [SerializeField]
    private GameOverUI gameOverUI;
    [SerializeField]
    private float uiDistance;

    private Canvas canvas;

    void Awake() {
        if (Instance != null) {
            Debug.LogError($"An instance of {nameof(GameOver)} already exists!");
            return;
        }

        Instance = this;
    }

    void Start() {
        if (transform.GetChild(0).TryGetComponent(out canvas)) {
            if (Camera.main == null) {
                Debug.LogWarning("Main camera not found");
            }

            canvas.worldCamera = Camera.main;
            canvas.planeDistance = uiDistance;
            canvas.gameObject.SetActive(false);
        }
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
