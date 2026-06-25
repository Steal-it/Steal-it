using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour {
    void Awake() {
        DOTween.Init(false, true, LogBehaviour.Verbose).SetCapacity(10, 10);
    }
}
