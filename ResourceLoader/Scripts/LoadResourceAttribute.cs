using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assign to an field in a class that implements IResourceLoader
/// Injects a resource into this field when an instance of this object is loaded.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class LoadResourceAttribute : Attribute
{
    /// <summary>
    /// The path to the resource.
    /// </summary>
    public string path;

    /// <summary>
    /// </summary>
    /// <param name="path">Starting from: "Resources/</param>
    public LoadResourceAttribute(string path)
    {
        this.path = path;
    }
}
