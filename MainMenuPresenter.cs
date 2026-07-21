using System;
using System.Collections.Generic;

namespace Monsterday.Core
{
    public static class ServiceRegistry
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static void Register<T>(T service) where T : class
        {
            Services[typeof(T)] = service ?? throw new ArgumentNullException(nameof(service));
        }

        public static T Get<T>() where T : class
        {
            if (Services.TryGetValue(typeof(T), out var service)) return (T)service;
            throw new InvalidOperationException($"Service {typeof(T).Name} wurde noch nicht registriert.");
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            if (Services.TryGetValue(typeof(T), out var value))
            {
                service = (T)value;
                return true;
            }

            service = null;
            return false;
        }

        public static void Clear() => Services.Clear();
    }
}
