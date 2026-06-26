using UnityEngine;
using Avatar = Ubiq.Avatars.Avatar;

public class AvatarLocale : MonoBehaviour {
    [SerializeField]
    private Avatar avatar;

    void Awake() {
        if (!avatar && !transform.root.TryGetComponent(out avatar)) {
            Debug.LogWarning("Avatar not found for" + gameObject.name);
        }
    }
    public bool IsLocal() {
        return avatar.IsLocal;
    }
}
