using Assets.Scripts.GameScripts.Lobby;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class LobbyAPIHandler
{
    private static readonly HttpClient client = new HttpClient();

    //private static readonly string _apiUrl = "http://lobby.cebt.dk/api/Lobby";

    //debug and testing purposes, run locally
    private static readonly string _apiUrl = "http://localhost:19255/api/Lobby";


    public static async Task<string> GetLobbiesAsync()
    {
        var response = await client.GetAsync(_apiUrl);
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }

    public static async Task<string> CreateLobbyAsync()
    {
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        var lobby = new Lobby() { LobbyId = 42, PlayerCount = 0 };
        var data = new LobbyPostMessage() { lobby = lobby, messageType = LobbyMessageType.CREATE };
        var jsonContent = JsonConvert.SerializeObject(data);
        using (var content = new StringContent(jsonContent, Encoding.UTF8, "application/json"))
        {
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = client.PostAsync(_apiUrl, content).Result;
            if(!response.IsSuccessStatusCode)
            {
                var message = response.Content.ReadAsStringAsync().Result;
                return message;
            }
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
            //var content = new ByteArrayContent(Encoding.Default.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(data)));
            //var values = new Dictionary<string, string>
            //{
            //    {"data", Newtonsoft.Json.JsonConvert.SerializeObject(data) }
            //};
    }
}
