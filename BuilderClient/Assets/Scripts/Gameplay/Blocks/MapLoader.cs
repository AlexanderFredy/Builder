using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    [SerializeField] private BlockMap _blockMap;
    private void Start()
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            {"mapId", MultiplayerManager.Instance.MapID }
        };
        WebRequest.Instance.Post(URILibrary.MAIN + URILibrary.GET_MAP_BLOCKS,data,Success,Error);
    }

    private void Success(string jsonArray)
    {
        string[] jsons = JsonUtility.FromJson<Wrapper>("{\"jsons\": " + jsonArray + "}").jsons;
        for (int i = 0; i < jsons.Length; i++)
        {
            _blockMap.SpawnBlockFromJSON(jsons[i]);
        }

        print("Map was loaded");
    }

    private void Error(string error)
    {
        Debug.LogError("During loading map was error: " + error);
    }

    [System.Serializable]
    private class Wrapper
    {
        public string[] jsons;
    }
}
