using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ruinborne.Core
{
    public static class GameEventBus
    {
        private static readonly Dictionary<Type, List<WeakReference>> _listeners = new();

        public static void Subscribe<T>(Action<T> listener) where T : struct
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var list))
            {
                list = new List<WeakReference>();
                _listeners[type] = list;
            }
            list.Add(new WeakReference(listener));
        }

        public static void Unsubscribe<T>(Action<T> listener) where T : struct
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var list))
                return;

            list.RemoveAll(wr => wr.Target is Action<T> t && t == listener);
        }

        public static void Publish<T>(T evt) where T : struct
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var list))
                return;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!list[i].IsAlive)
                {
                    list.RemoveAt(i);
                    continue;
                }

                if (list[i].Target is Action<T> listener)
                    listener(evt);
            }
        }

        public static void Clear<T>() where T : struct
        {
            _listeners.Remove(typeof(T));
        }

        public static void ClearAll()
        {
            _listeners.Clear();
        }
    }
}
