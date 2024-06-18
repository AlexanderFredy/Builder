using GameDevWare.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class OtherMapManager : MonoBehaviour
{
    [SerializeField] private MatchmakingManager _matchmakingManager;
    [SerializeField] private MapButton _mapButtonPrefab;
    [SerializeField] private Transform _buttonPanel;
    [SerializeField] private GameObject _mainCanvas;

    private Dictionary<string, MapButton> _buttons = new Dictionary<string, MapButton>();
    private string _mapID;

    public void SetMapId(string mapID)
    {
        _mapID = mapID;
    }

    public void ConnectById() => ClickButton(_mapID);

    public async void ClickButton(string mapID)
    {
        _mainCanvas.SetActive(false);
        bool success = await _matchmakingManager.TryConnectByID(mapID);
        if (success == false) _mainCanvas.SetActive(true);
        print("coonect: " + success);
    }

    private void Start()
    {    
        MultiplayerManager.Instance.Plus += Plus;
        MultiplayerManager.Instance.Minus += RemoveButton;
        LoadAvailable();
    }

    private async void LoadAvailable()
    {
        var rooms = await MultiplayerManager.Instance.GetAvailableRooms();
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].name == "lobby") continue;
            CreateButton(rooms[i].roomId);
        }
    }

    private void Plus(List<object> objs)
    {
        var data = (IndexedDictionary<string, object>) objs[1];
        if (data["name"].ToString() == "lobby") return;
        
        string roomID = (string)objs[0];
        CreateButton(roomID);
    }

    private void CreateButton(string roomID)
    {
        if (_buttons.ContainsKey(roomID)) return;
        var button = Instantiate(_mapButtonPrefab, _buttonPanel);
        button.Init(roomID, ClickButton);
        _buttons.Add(roomID, button);
    }

    private void RemoveButton(string roomID)
    {
        if (_buttons.ContainsKey(roomID) == false) return;
        Destroy(_buttons[roomID].gameObject);
        _buttons.Remove(roomID);
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.Plus -= Plus;
        MultiplayerManager.Instance.Minus -= RemoveButton;
    }
}
