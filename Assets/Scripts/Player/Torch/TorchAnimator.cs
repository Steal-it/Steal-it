using System.Collections;
using UnityEngine;

public class TorchAnimator : MonoBehaviour {

    [SerializeField]
    private Transform torchAttachPoint;
    [SerializeField]
    private Transform pocketAttachPoint;
    [SerializeField]
    private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField]
    private float duration = 1f;

    private Coroutine moveCoroutine;

    void Start() {
        transform.SetPositionAndRotation(torchAttachPoint.position, torchAttachPoint.rotation);
    }

    public void TorchVisible(bool _value) {
        print(_value);
        if (moveCoroutine != null) {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(MoveTo(_value ? torchAttachPoint : pocketAttachPoint));
    }

    IEnumerator MoveTo(Transform _target) {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curvedT = curve.Evaluate(t);
            transform.position = Vector3.Lerp(startPos, _target.position, curvedT);
            yield return null;
        }

        transform.position = _target.position;
    }

}
