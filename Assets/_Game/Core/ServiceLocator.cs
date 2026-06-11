using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ruinborne.Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _services.Clear();
        }

        public static void Register<T>(T service) where T : class
        {
            _services[typeof(T)] = service;
        }

        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;

            Debug.LogError($"[ServiceLocator] Service not found: {typeof(T).Name}");
            return null;
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj))
            {
                service = (T)obj;
                return true;
            }
            service = null;
            return false;
        }

        public static void Unregister<T>() where T : class
        {
            _services.Remove(typeof(T));
        }
    }
}
