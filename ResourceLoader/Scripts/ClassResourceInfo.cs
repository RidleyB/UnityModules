using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Contains the ResourceInfo for a single type.
/// </summary>
public class ClassResourceInfo
{
    /// <summary>
    /// The type for this class.
    /// </summary>
    public Type type { get; private set; } = null;

    /// <summary>
    /// The list of reference to resource info
    /// </summary>
    public List<ResourceInfo> resourceReferences { get; private set; } = null;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="type"></param>
    public ClassResourceInfo(Type type)
    {
        this.type = type;
        this.resourceReferences = new List<ResourceInfo>();
    }

    /// <summary>
    /// Injects a passed in object with the resources for it's class type.
    /// </summary>
    /// <param name="obj"></param>
    public void InjectResources(UnityEngine.Object obj)
    {
        foreach (ResourceInfo resource in resourceReferences)
            resource.InjectResource(obj);
    }
}

/// <summary>
/// Contains data to load and inject a resource into a field.
/// </summary>
public class ResourceInfo
{
    /// <summary>
    /// The field to inject with the resource.
    /// </summary>
    private readonly FieldInfo fieldInfo = null;

    /// <summary>
    /// The path to the resource.
    /// </summary>
    private readonly string path = null;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="path">The path to the resource.</param>
    /// <param name="fieldInfo">The field to inject the resource to.</param>
    public ResourceInfo(string path, FieldInfo fieldInfo)
    {
        this.path = path;
        this.fieldInfo = fieldInfo;
    }

    /// <summary>
    /// Injects the resource into the given object's field.
    /// </summary>
    /// <param name="obj"></param>
    public void InjectResource(UnityEngine.Object obj)
    {
        try
        {
            fieldInfo.SetValue(obj, GameObject.Instantiate(LoadResource()));
        }
        catch(Exception ex)
        {
            Debug.LogError(obj.name + "." + obj.GetType() + ": " + ex.Message);
        }
    }

    /// <summary>
    /// Loads the resource from Unity's Resource.Load. 
    /// Uses the fieldInfo type for the type of resource to load.
    /// </summary>
    /// <returns></returns>
    public UnityEngine.Object LoadResource() => Resources.Load(path, fieldInfo.FieldType) ?? throw new Exception("Could not find resource. \nPath: Resources/" + path);
}