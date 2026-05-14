using System;
using UnityEngine.InputSystem;

public interface IControllerConfigurator {
  public void Enable(Action<InputAction.CallbackContext> _action);

  public void Disable(Action<InputAction.CallbackContext> _action);
}