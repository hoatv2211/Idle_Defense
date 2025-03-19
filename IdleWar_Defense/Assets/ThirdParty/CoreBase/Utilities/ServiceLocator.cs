using System;
using System.Collections.Generic;

namespace Beemob.Utilities
{
    public class ServiceLocator
    {
        private readonly IDictionary<Type, object> _instantiatedServices;
        private static ServiceLocator _instance;
        private static ServiceLocator Instance => _instance ?? (_instance = new ServiceLocator()); // auto set;

        private ServiceLocator()
        {
            _instantiatedServices = new Dictionary<Type, object>();
        }
        public static T GetService<T>()
        {
            var key = typeof(T);
            if (!Instance._instantiatedServices.ContainsKey(key))
            {
                throw new ArgumentException($"Type '{key.Name}' has not been registered.");
            }
            return (T) Instance._instantiatedServices[key];
        }
        public static void Register<T>(T service)
        {
            var key = typeof(T);
            if (!Instance._instantiatedServices.ContainsKey(key))
            {
                Instance._instantiatedServices.Add(key, service);
            }
            else 
            {
                Instance._instantiatedServices[key] = service;
            }
        }

        public static bool Contain<T>()
        {
            return Instance._instantiatedServices.ContainsKey(typeof(T));
        }
        public static void Clear()
        {
            Instance._instantiatedServices.Clear();
        }
    }
}