using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A static class for general helpful methods
/// </summary>
public static class Helpers
{

    /// <summary>
    /// Destroy all child objects of this transform (Unintentionally evil sounding)
    /// Use it like so:
    /// <code>
    /// transform.DestroyChildren()
    /// </code>
    /// </summary>
    public static void DestroyChildren(this Transform transform){
        foreach (Transform child in transform)
        {
            Object.Destroy(child.gameObject);
        }
    }
}
