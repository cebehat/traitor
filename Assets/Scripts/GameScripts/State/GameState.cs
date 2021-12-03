using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine;

namespace Cebt
{
    [RequireComponent(typeof(MultiplayerHandler))]
    [RequireComponent(typeof(MenuHandler))]
    public class GameState : NetworkBehaviour
    {
        //first menu where you select host or join
        //then if host a menu showing connected clients
        //or if join that shows host and other clients

        private MenuHandler menuHandler;
        private MultiplayerHandler multiplayerHandler;
        private Camera menuCamera;

        private void Start()
        {
            multiplayerHandler = GetComponent<MultiplayerHandler>();
            menuHandler = GetComponent<MenuHandler>();

            menuHandler.multiplayerHandler = multiplayerHandler;
            menuCamera = GetComponentInChildren<Camera>();
        }

        public static void Disconnect()
        {
            NetworkManager.Singleton.Shutdown();
        }

        public void StartGameRound()
        {
            menuCamera.enabled = false;
            multiplayerHandler.StartGame();
        }

        void OnDestroy()
        {
            //Disconnect();
        }
    }
}
