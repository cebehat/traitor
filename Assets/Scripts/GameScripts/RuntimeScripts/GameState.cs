using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine;

namespace Cebt
{
    public class GameState : NetworkBehaviour
    {
        private static string ip = "127.0.0.1";

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            GUILayout.EndArea();
        }

        static void StartButtons()
        {
            ip = GUILayout.TextField(ip);
            if (GUILayout.Button("Host"))
            {
                NetworkManager.Singleton.StartHost();
            }
            if (GUILayout.Button("Client")) {
                NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = ip;
                NetworkManager.Singleton.StartClient(); 
            }
            if (GUILayout.Button("Server")) 
            {
                NetworkManager.Singleton.StartServer();
            }
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);

            if (GUILayout.Button("Disconnect")) Disconnect();
        }

        public static void Disconnect()
        {
            NetworkManager.Singleton.Shutdown();
        }

        void OnDestroy()
        {
            //Disconnect();
        }
    }
}
