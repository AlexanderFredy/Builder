using System.Collections.Generic;
using UnityEngine;

public class MyMapManager : MonoBehaviour
{
    [SerializeField] private MapButton _myMapButtonPrefab;
    [SerializeField] private RectTransform _buttonPanel;

    void Start()
    {
        string uri = URILibrary.MAIN + URILibrary.GET_MY_MAPS;
        var data = new Dictionary<string, string>() { { "userid", UserInfo.Instance.id } };
        WebRequest.Instance.Post(uri, data, CompleteLoad, ErrorLoad);
    }

    private void CompleteLoad(string json)
    {
        //print("Succses response from server: " + json);
        string[] mapids = JsonUtility.FromJson<Wrapper>(json).mapids;

        for (int i = 0; i < mapids.Length; i++)
        {
            Instantiate(_myMapButtonPrefab, _buttonPanel).Init(mapids[i], ClickMapButton);
        }
    }

    private void ErrorLoad(string error)
    {
        print("Error of loading rooms: " + error);
    }

    [SerializeField] private GameObject _mainCanvas;
    [SerializeField] private GameObject _setPrivateCanvas;
    [SerializeField] private MatchmakingManager _matchmakingManager;

    public void ClickMapButton(string mapID)
    {
        _mainCanvas.SetActive(false);
        _setPrivateCanvas.SetActive(true);
        _matchmakingManager.SetMapID(mapID);
    }


    [System.Serializable]
    public class Wrapper
    {
        public string[] mapids;
    }
}
