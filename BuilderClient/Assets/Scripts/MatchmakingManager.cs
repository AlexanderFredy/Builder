using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class MatchmakingManager : MonoBehaviour
{
    [SerializeField] private string _gameSceneName;
    private const int roomIdLength = 7;
    private string _mapID = string.Empty;

    public void SetMapID(string mapID)
    {
        if (string.IsNullOrWhiteSpace(mapID))
            mapID = GetRandomString(roomIdLength);
        _mapID = mapID;
    }

    public async void StartGame(bool isPrivate)
    {
        string mapID = _mapID;
        if (await TryConnectByID(mapID)) return;
        bool connect = await MultiplayerManager.Instance.TryCreateGame(isPrivate, mapID);
        if (connect) SceneManager.LoadScene(_gameSceneName);
    }

    public async Task<bool> TryConnectByID(string mapID)
    {
        bool connect = await MultiplayerManager.Instance.TryConnectGame(mapID);
        if (connect) SceneManager.LoadScene(_gameSceneName);
        return connect;
    }

    private string GetRandomString(int length)
    {
        string str = string.Empty;
        for (int i = 0; i < length; i++)
        {
            int random = UnityEngine.Random.Range(0, 36);
            if (random < 26) str += (char)(random + 65);
            else str += (random - 26).ToString();
        }

        return str;
    }
}
