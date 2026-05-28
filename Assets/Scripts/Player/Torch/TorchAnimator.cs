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
        float fullDistance = Vector3.Distance(torchAttachPoint.position, pocketAttachPoint.position);
        float currentDistance = Vector3.Distance(startPos, _target.position);
        float scaledDuration = duration * (currentDistance / fullDistance);

        float elapsed = 0f;

        while (elapsed < scaledDuration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scaledDuration);
            transform.position = Vector3.Lerp(startPos, _target.position, curve.Evaluate(t));
            yield return null;
        }

        transform.position = _target.position;
    }

}
