using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine;

public class LobbyServerComponent : NetworkBehaviour
{
    private NetworkList<LobbyData> lobbyIds;

    private void Awake()
    {
        lobbyIds = new NetworkList<LobbyData>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
