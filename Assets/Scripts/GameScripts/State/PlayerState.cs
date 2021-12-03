using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class PlayerState : NetworkBehaviour

{
    [SerializeField]
    public string PlayerName { get; set; }
    public ulong PlayerId => OwnerClientId;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnCharacter()
    {

    }
}
