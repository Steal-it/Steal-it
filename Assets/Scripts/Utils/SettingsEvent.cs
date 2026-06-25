// Assets/Scripts/EventBus/GameEvent.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class SettingsEvent<T> {
    private readonly List<Action<T>> listeners = new();

    public void Invoke(T _value) {

        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i]?.Invoke(_value);
    }

    public void Register(Action<T> _listener) => listeners.Add(_listener);
    public void Unregister(Action<T> _listener) => listeners.Remove(_listener);
}