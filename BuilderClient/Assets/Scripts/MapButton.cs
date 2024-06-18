using System;
using UnityEngine;
using UnityEngine.UI;

public class MapButton : MonoBehaviour
{
    [SerializeField] private Text _text;
    private string _mapId;
    private event Action<string> _onClick;

    public void Init(string mapid, Action<string> onClick)
    {
        _mapId = mapid;
        _text.text = _mapId;
        _onClick = onClick;
    }

    public void Click()
    {
        _onClick?.Invoke(_mapId);
    }
}
