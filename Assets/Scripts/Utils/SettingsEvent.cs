// Assets/Scripts/EventBus/GameEvent.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class SettingsEvent<T> {
    private readonly List<Action<T>> listeners = new();

    public void Invoke(T value) {

        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i]?.Invoke(value);
    }

    public void Register(Action<T> listener) => listeners.Add(listener);
    public void Unregister(Action<T> listener) => listeners.Remove(listener);
}