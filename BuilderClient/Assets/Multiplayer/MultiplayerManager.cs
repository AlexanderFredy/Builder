using Colyseus;
using GameDevWare.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MultiplayerManager : ColyseusManager<MultiplayerManager>
{
    private const string LobbyName = "lobby";
    private const string RoomStateName = "custom_room";

    #region Lobby
    private ColyseusRoom<LobbyState> _lobby;
    
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

        InitGameRoom(await Instance.client.Create<RoomState>(RoomStateName,data), mapID);
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
            InitGameRoom(await Instance.client.JoinById<RoomState>(mapID, data), mapID);
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
    #endregion


    #region GameRoom
    private const string ApplyBlockSpawnName = "inbox_block_spawn";
    private const string ApplyBlockDestroyName = "inbox_block_destroy";

    private ColyseusRoom<RoomState> _room;
    public event Action<string, Player> OnAddPlayer;
    public event Action<string, Player> OnRemovePlayer;
    public event Action<string> ApplyBlockSpawn;
    public event Action<string> ApplyBlockDestroy;

    public string SessionID { get; private set; }
    public string MapID { get; private set; }

    private void InitGameRoom(ColyseusRoom<RoomState> room, string mapID)
    {
        LeaveGame();

        room.State.players.OnAdd += (key, value) => OnAddPlayer?.Invoke(key, value);
        room.State.players.OnRemove += (key, value) => OnRemovePlayer?.Invoke(key, value);
        room.OnMessage<string>(ApplyBlockSpawnName, (s) => ApplyBlockSpawn?.Invoke(s));
        room.OnMessage<string>(ApplyBlockDestroyName, (s) => ApplyBlockDestroy?.Invoke(s));
        SessionID = room.SessionId;
        MapID = mapID;
        _room = room;
    }

    public void ProcessMapPlayer(Action<string, Player> ApplyMethodToPlayer)
    {
        if(_room == null)
        {
            Debug.LogError("There is not a room, but players try update");
            return;
        }
        _room.State.players.ForEach(ApplyMethodToPlayer);
    }

    public void SendMessage(string key, Dictionary<string,object> data)
    {
        _room.Send(key, data);
    }

    public void SendMessage(string key, string data)
    {
        _room.Send(key, data);
    }

    public void LeaveGame()
    {
        if (_room != null) _room.Leave();
    }
    #endregion
}
