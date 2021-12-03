using Cebt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public MultiplayerHandler multiplayerHandler;
    private MenuState state;
    private string GameCode = "";

    

    // Start is called before the first frame update
    void OnGUI()
    {
        switch (state)
        {
            case MenuState.START:
                StartMenu();
                break;
            case MenuState.MULTIPLAYER:
                MultiplayerMenu();
                break;
            case MenuState.LOBBY:
                LobbyMenu();
                break;
            case MenuState.SETTINGS:
                SettingsMenu();
                break;
            case MenuState.GAMEPLAY:
                break;
            case MenuState.PAUSE:
                break;
            default:
                break;
        }
    }

    void StartMenu()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (GUILayout.Button("Multiplayer"))
        {
            state = MenuState.MULTIPLAYER;
        }
        if (GUILayout.Button("Settings"))
        {
            state = MenuState.SETTINGS;
        }
        if (GUILayout.Button("Quit"))
        {
            Application.Quit();
        }
        GUILayout.EndArea();
    }

    void MultiplayerMenu()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (GUILayout.Button("Host"))
        {
            GameCode = multiplayerHandler.HostGame();
            state = MenuState.LOBBY;
        }
        GameCode = GUILayout.TextField(GameCode);
        if (GUILayout.Button("Join"))
        {
            string message;
            if (multiplayerHandler.JoinGame(GameCode, out message))
            {
                state = MenuState.LOBBY;
            }
            else
            {
                state = MenuState.MULTIPLAYER;
            }
            
        }
        if (GUILayout.Button("Back"))
        {
            state = MenuState.START;
        }
        GUILayout.EndArea();
    }

    void LobbyMenu()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        GUILayout.Label("Game Code: " + GameCode);
        GUILayout.Label("Players in Lobby: " + multiplayerHandler.LobbyPlayerCount);
        if(GUILayout.Button("Start Game"))
        {
            FindObjectOfType<GameState>().StartGameRound();
        }
        if (GUILayout.Button("Leave Game"))
        {
            multiplayerHandler.LeaveGame();
            GameCode = "";
            state = MenuState.MULTIPLAYER;
        }
        GUILayout.EndArea();
    }

    void SettingsMenu()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        GUILayout.EndArea();
    }

    private enum MenuState
    {
        START,
        MULTIPLAYER,
        LOBBY,
        SETTINGS,
        GAMEPLAY,
        PAUSE
    }
}
