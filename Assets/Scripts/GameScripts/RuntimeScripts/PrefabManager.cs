using UnityEngine;
using System;
using System.Collections.Generic;
using Cebt.Shared;

public class PrefabManager : MonoBehaviour
{
    public GameObject GetPrefab(string prefabType, string prefabVariation)
    {
        try
        {
            var prefabPath = String.Format("Prefabs/{0}/{1}", prefabType, prefabVariation);
            var resource = (Resources.Load(prefabPath) as GameObject);
            return resource;
        }
        catch(Exception e)
        {
            throw e;
        }
    }

}