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

    //[Header("Room components")]
    //[SerializeField]
    //public List<RoomComponent> _roomComponentKeys = new List<RoomComponent>();
    //[SerializeField]
    //public List<GameObject> _roomComponentValues = new List<GameObject>();

    //[Header("")]

    ////Unity doesn't know how to serialize a Dictionary
    //[SerializeField]
    //public Dictionary<RoomComponent, GameObject> roomComponentDictionary = new Dictionary<RoomComponent, GameObject>();

    //public void OnBeforeSerialize()
    //{
    //    //_keys.Clear();
    //    //_values.Clear();

    //    //foreach (var kvp in _myDictionary)
    //    //{
    //    //    _keys.Add(kvp.Key);
    //    //    _values.Add(kvp.Value);
    //    //}
    //}

    //public void OnAfterDeserialize()
    //{
    //    roomComponentDictionary = new Dictionary<RoomComponent, GameObject>();

    //    for (int i = 0; i != Math.Min(_roomComponentKeys.Count, _roomComponentValues.Count); i++)
    //        roomComponentDictionary.Add(_roomComponentKeys[i], _roomComponentValues[i]);
    //}

    //void OnGUI()
    //{
    //    foreach (var kvp in _myDictionary)
    //        GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);
    //}

}