using UnityEngine;

public class MonsterAnimator : MonoBehaviour {
    [SerializeField]
    private Animator animator;

    private const string IS_STUNNED_ANIM_VAR = "isStunned";

    public void SetIsStunned(bool _value) {
        animator.SetBool(IS_STUNNED_ANIM_VAR, _value);
    }
}
