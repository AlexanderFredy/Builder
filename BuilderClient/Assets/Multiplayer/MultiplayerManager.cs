using Colyseus;
using GameDevWare.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MultiplayerManager : ColyseusManager<MultiplayerManager>
{
    private const string LobbyName = "lobby";
    private const string RoomStateName = "custom_room";

    private ColyseusRoom<LobbyState> _lobby;
    private ColyseusRoom<RoomState> _room;

    public event Action<List<IndexedDictionary<string, object>>> Rooms;
    public event Action<List<object>> Plus;
    public event Action<string> Minus;

    protected override async void Start()
    {
        if (Instance != this) return;
        Instance.InitializeClient();

        print("join to lobby");
        _lobby = await Instance.client.JoinOrCreate<LobbyState>(LobbyName);

        _lobby.OnMessage<List<IndexedDictionary<string, object>>>("rooms",(list) => Rooms?.Invoke(list));
        _lobby.OnMessage<List<object>>("+", (list) => Plus?.Invoke(list));
        _lobby.OnMessage<string>("-", (str) => Minus?.Invoke(str));            
    }

    public async Task<bool> TryCreateGame(bool isPrivate, string mapID)
    {
        var data = new Dictionary<string, object>()
        {
            {"isPrivate", isPrivate },
            {"mapID", mapID },
            {"userid", UserInfo.Instance.id}
        };
        
        _room = await Instance.client.Create<RoomState>(RoomStateName,data);
        return true;
    }

    public async Task<bool> TryConnectGame(string mapID)
    {
        var data = new Dictionary<string, object>()
        {
            {"userid", UserInfo.Instance.id}
        };

        try
        {
            _room = await Instance.client.JoinById<RoomState>(mapID, data);
            print("join to room");
            return true;
        } catch (Exception ex)
        {
            print("Try to connect is faild: " + ex.ToString());
            return false;
        } 
    }

    public async Task<ColyseusRoomAvailable[]> GetAvailableRooms()
    {
        while (true)
        {
            if (_lobby == null) await Task.Delay(1000);
            else break;
        }

        return await client.GetAvailableRooms();
    }
}
