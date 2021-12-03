using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using PopShark.LobbyApiInterface;
using Unity.Netcode.Transports.UNET;
[RequireComponent(typeof(NetworkObject))]
public class MultiplayerHandler : NetworkBehaviour
{
    [SerializeField]
    private string ApiUrl = @"https://lobbyrestapi20211016183500.azurewebsites.net/";

    private RequestHandler RequestHandler;

    NetworkVariable<int> PlayerCount = new NetworkVariable<int>(NetworkVariableReadPermission.Everyone, 0);

    public static string GameCode { get; private set; } = "";
    public int LobbyPlayerCount => PlayerCount.Value;

    
    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerCountServerRpc()
    {
        PlayerCount.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }

    

    private void Start()
    {
        RequestHandler = new RequestHandler(ApiUrl);
        NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        UpdatePlayerCountServerRpc();
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        UpdatePlayerCountServerRpc();
    }

    private void OnConnectedToServer()
    {
        UpdatePlayerCountServerRpc();
    }

    public string HostGame()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            GameCode = RequestHandler.Create();
        }
        return GameCode;
    }

    public void StartGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            RequestHandler.Delete(GameCode);
        }
        //start actual game
    }

    public bool JoinGame(string gameCode, out string message)
    {
        GameCode = gameCode;
        message = "";
        int.TryParse(gameCode, out int id);
        string ip = RequestHandler.GetHost(id);
        NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = ip;
        return NetworkManager.Singleton.StartClient();
    }

    public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            RequestHandler.Delete(GameCode);
        }
        if(NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
