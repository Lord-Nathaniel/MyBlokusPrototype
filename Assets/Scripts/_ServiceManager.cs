using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class allows every managers to register in this dictionary and be called from any other manager without direct call.
/// It will not be destroyed on load and will allow music to continue from a scene to another.
/// -IN- All Managers
/// </summary>
[DefaultExecutionOrder(-1000)]
public class ServiceManager : MonoBehaviour
{
    private static readonly Dictionary<Type, object> services = new();

    private static ServiceManager Instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance == null)
        {
            var gameObject = new GameObject("[ServiceLocator]");
            Instance = gameObject.AddComponent<ServiceManager>();
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Allows the service to be registered during the game initialization and called without direct reference.
    /// Put this in the Awake() method : ServiceLocator.Register(this);
    /// Put this in the OnDestroy() method : ServiceLocator.Unregister<ServiceName>();
    /// </summary>
    public static void Register<T>(T service)
    {
        var type = typeof(T);
        if (services.ContainsKey(type))
            services[type] = service;
        else
            services.Add(type, service);
    }

    /// <summary>
    /// Allows the service to be unregistered when destroyed.
    /// Put this in the Awake() method : ServiceLocator.Register(this);
    /// Put this in the OnDestroy() method : ServiceLocator.Unregister<ServiceName>();
    /// </summary>
    public static void Unregister<T>()
    {
        var type = typeof(T);
        services.Remove(type);
    }


    /// <summary>
    /// Allows the usage of a service method without direct call.
    /// Put this in when a service method is needed : ServiceLocator.Get<ServiceName>()?.Play("ServiceMethod");
    /// </summary>
    public static T Get<T>()
    {
        var type = typeof(T);
        if (services.TryGetValue(type, out var service))
            return (T)service;

        Debug.LogWarning($"[ServiceLocator] Service of type {type.Name} not found!");
        return default;
    }

    private void OnDestroy()
    {
        services.Clear();
    }
}
