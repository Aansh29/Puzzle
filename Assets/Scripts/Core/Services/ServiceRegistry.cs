using System;
using System.Collections.Generic;

namespace Puzzle.Services
{
    public static class ServiceRegistry
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static void Register<T>(T service) where T : class
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            Services[typeof(T)] = service;
        }

        public static T Get<T>() where T : class
        {
            if (Services.TryGetValue(typeof(T), out object service))
            {
                return service as T;
            }

            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.");
        }

        public static bool Has<T>() where T : class
        {
            return Services.ContainsKey(typeof(T));
        }

        public static void Unregister<T>() where T : class
        {
            Services.Remove(typeof(T));
        }

        public static void Clear()
        {
            Services.Clear();
        }
    }
}