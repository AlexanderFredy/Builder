using UnityEngine;
using UnityEngine.UI;

public class BlockUI : MonoBehaviour
{
    [SerializeField] private BlockManager _manager;
    [SerializeField] private Image[] _images;

    private void Start()
    {
        _manager.UpdateCurrentIndex += SetImages;
        SetImages(_manager.CurrentIndex);
    }

    private void SetImages(int centerBlockIndex)
    {
        int centerIndex = _images.Length / 2;
        if (_images.Length % 2 == 0) centerIndex--;

        for (int i = 0; i < _images.Length; i++)
        {
            int blockIndex = centerBlockIndex + (i - centerIndex);
            _images[i].sprite = _manager.GetSprite(blockIndex);
        }
    }

    private void OnDestroy()
    {
        _manager.UpdateCurrentIndex -= SetImages;
    }
}
