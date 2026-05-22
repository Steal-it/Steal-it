using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface ITorchControllerConfigurator {
  public void Enable(Transform _torchTransform, Action<InputAction.CallbackContext> _action);

  public void Disable(Action<InputAction.CallbackContext> _action);
}