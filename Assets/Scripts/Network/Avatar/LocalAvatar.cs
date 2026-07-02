using UnityEngine;
using Avatar = Ubiq.Avatars.Avatar;

public class LocalAvatar : MonoBehaviour {
    public Avatar avatar;
    public bool IsLocal => avatar.IsLocal;

    protected virtual void Awake() {
        avatar = transform.root.GetComponent<Avatar>();
    }

}