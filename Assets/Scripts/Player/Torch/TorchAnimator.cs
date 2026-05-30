using System.Collections;
using UnityEngine;

public class TorchAnimator : MonoBehaviour {

    [SerializeField]
    private Transform torchAttachPoint;
    [SerializeField]
    private Transform pocketAttachPoint;
    [SerializeField]
    private TorchLight torchLight;
    [SerializeField]
    private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField]
    private float duration = 1f;

    private Coroutine moveCoroutine;

    void Start() {
        transform.SetPositionAndRotation(torchAttachPoint.position, torchAttachPoint.rotation);
    }

    public void ToggleTorchVisible(bool _value) {
        if (moveCoroutine != null) {
            StopCoroutine(moveCoroutine);
        }

        torchLight.InPocket(!_value);

        moveCoroutine = StartCoroutine(MoveTo(_value ? torchAttachPoint : pocketAttachPoint));
    }

    IEnumerator MoveTo(Transform _target) {
        Vector3 startLocalPos = transform.localPosition;
        Vector3 targetLocalPos = _target.localPosition;

        float fullDistance = Vector3.Distance(torchAttachPoint.localPosition, pocketAttachPoint.localPosition);
        float currentDistance = Vector3.Distance(startLocalPos, targetLocalPos);

        float scaledDuration = duration * (currentDistance / fullDistance);
        float elapsed = 0f;

        while (elapsed < scaledDuration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scaledDuration);
            transform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, curve.Evaluate(t));
            yield return null;
        }

        transform.localPosition = targetLocalPos;
    }

}
