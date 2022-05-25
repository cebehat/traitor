using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cebt.Shared;

public class RoomBehavior : MonoBehaviour
{

    private PrefabManager prefabManager;
    private Room roomData;


    private void Awake()
    {
        prefabManager = FindObjectOfType<PrefabManager>();
    }

    private void Start()
    {
        
        //var collider = gameObject.AddComponent<BoxCollider>();
        //collider.isTrigger = true;
        //collider.size = new Vector3(10, 4, 10);
    }

    //void OnTriggerEnter(Collider coll){
    //    if(coll.CompareTag("Player")){
    //        //Debug.Log("Player Entered");
    //    }
    //}
}
