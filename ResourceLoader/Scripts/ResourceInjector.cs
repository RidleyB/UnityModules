using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Injects resources after a scene load using attributes.
/// Looks for attributes in classes that implement the IResourceLoaded interface.
/// </summary>
public static class ResourceInjector
{
    #region Initialization

    private static bool shouldReload = true;

    /// <summary>
    /// Configures the c
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void StaticConfiguration()
    {
        if(shouldReload)
        {
            resources = new Dictionary<string, ResourceInfo>();
            classResources = new Dictionary<Type, ClassResourceInfo>();

            Application.quitting += OnApplicationQuit;
            SceneManager.sceneLoaded += GetResourceInfo;
            SceneManager.sceneLoaded += InjectResources;
        }
    }

    private static void OnApplicationQuit()
    {
        Application.quitting -= OnApplicationQuit;
        SceneManager.sceneLoaded -= InjectResources;
        SceneManager.sceneLoaded -= GetResourceInfo;
    }

    #endregion

    /// <summary>
    /// Dictionary of class resources with a type key - for easy injection
    /// </summary>
    private static Dictionary<Type, ClassResourceInfo> classResources = null;

    /// <summary>
    /// List of resources to their paths - to ensure we only add each resource once.
    /// </summary>
    private static Dictionary<string, ResourceInfo> resources = null;

    /// <summary>
    /// Generates ClassResourceInfo and ResourceInfo
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="sceneMode"></param>
    private static void GetResourceInfo(Scene scene, LoadSceneMode sceneMode)
    {
        LoadResourceAttribute attr;
        ClassResourceInfo classResource = null;
        ResourceInfo resource = null;

        foreach (Type t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => typeof(IResourceLoader).IsAssignableFrom(t))))
        {
            if (!classResources.ContainsKey(t))
            {
                classResource = new ClassResourceInfo(t);
                classResources.Add(t, classResource);

                foreach (FieldInfo fieldInfo in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    attr = fieldInfo.GetCustomAttribute<LoadResourceAttribute>();
                    if (attr != null)
                    {
                        if (!resources.ContainsKey(attr.path))
                        {
                            resource = new ResourceInfo(attr.path, fieldInfo);
                            resources.Add(attr.path, resource);
                        }

                        classResources[t].resourceReferences.Add(resources[attr.path]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Injects the stored resources into the new loaded scene objects.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="sceneMode"></param>
    private static void InjectResources(Scene scene, LoadSceneMode sceneMode)
    {
        foreach(IResourceLoader resourceLoader in scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<IResourceLoader>(true)))
        {
            if(classResources.ContainsKey(resourceLoader.GetType()))
                classResources[resourceLoader.GetType()].InjectResources(resourceLoader as UnityEngine.Object);
        }
    }
}
