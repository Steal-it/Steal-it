using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour {
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private RedScreenOfDeath redScreenOfDeath;
    [SerializeField]
    private TMP_Text winnerText;
    [SerializeField]
    private TMP_Text loserText;
    [SerializeField]
    private float uiDistance = 0.4f;

    void Start() {
        if (Camera.main == null) {
            Debug.LogWarning("Main camera not found");
        }
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = uiDistance;


    }

    public float ActivatePanel(bool _isWinner) {
        redScreenOfDeath.gameObject.SetActive(false);

        gameObject.SetActive(true);
        winnerText.gameObject.SetActive(_isWinner);
        loserText.gameObject.SetActive(!_isWinner);

        return animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
    }
}
