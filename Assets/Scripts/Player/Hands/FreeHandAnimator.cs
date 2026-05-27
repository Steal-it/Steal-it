using UnityEngine;

public class FreeHandAnimator : MonoBehaviour {
    [SerializeField]
    [Tooltip("Speed at which the hand model will change grip strength in units/second. A speed of 2 will change from 0 (no grip) to 1 (full grip) in 0.5 seconds, for example. Set to 0 to disable smoothing")]
    private float smoothingSpeed = 4;
    [SerializeField]
    private float pokeAnimationOffset;


    private Animator animator;
    public float TargetGrip { private get; set; }
    public float TargetPoke { private get; set; }
    private float currentGrip;
    private float currentPoke;
    private bool torch = false;

    private static readonly int torchProperty = Animator.StringToHash("InTorchPosition");
    private static readonly int gripProperty = Animator.StringToHash("Grabbing");
    private static readonly int pokeProperty = Animator.StringToHash("Pokeing");
    private static readonly int completedProperty = Animator.StringToHash("CompletedGrab");

    void Awake() {
        TryGetComponent(out animator);
    }

    private void Update() {
        if (torch) return;

        if (!Mathf.Approximately(currentGrip, TargetGrip)) {
            var delta = smoothingSpeed * Time.deltaTime;
            currentGrip = Mathf.MoveTowards(currentGrip, TargetGrip, delta);
            animator.SetFloat(gripProperty, currentGrip);
            if (currentGrip == 1) {
                animator.SetBool(completedProperty, true);
            } else {
                animator.SetBool(completedProperty, false);
            }
        }
        if (!Mathf.Approximately(currentPoke, TargetPoke)) {
            var delta = smoothingSpeed * Time.deltaTime;
            currentPoke = Mathf.MoveTowards(currentPoke, TargetPoke, delta);
            animator.SetFloat(pokeProperty, currentPoke);
        }
    }

    public void GetInTorchPosition(bool _value) {
        animator.SetBool(torchProperty, _value);
        torch = _value;
    }

}
