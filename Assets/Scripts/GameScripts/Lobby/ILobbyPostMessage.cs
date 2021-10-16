using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameScripts.Lobby
{
    interface ILobbyPostMessage
    {
        public LobbyMessageType messageType { get; set; }
        public Lobby lobby { get; set; }
    }
}
