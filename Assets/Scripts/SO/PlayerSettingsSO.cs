using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Scriptable Objects/PlayerSettings")]
public class PlayerSettingsSO : ScriptableObject {
    [Header("Game preferences")]
    public Side playerTorchHand;

    private SettingsEvent<Side> _onPlayerTorchChanged;
    public SettingsEvent<Side> OnPlayerTorchChanged
        => _onPlayerTorchChanged ??= new SettingsEvent<Side>();

    public void SetPlayerTorchHand(Side _newSide) {
        playerTorchHand = _newSide;
        OnPlayerTorchChanged.Invoke(_newSide);
    }
}
