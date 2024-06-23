using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [SerializeField] private Block[] _blocks;
    [SerializeField] private Sprite[] _blockSprites;
    public event Action<int> UpdateCurrentIndex;

    public int CurrentIndex { get; private set; } = 0;

    public Block GetBlock(int index)
    {
        FixIndex(ref index);
        return _blocks[index];
    }

    public Sprite GetSprite(int index)
    {
        FixIndex(ref index);
        return _blockSprites[index];
    }

    private void FixIndex(ref int index)
    {
        if (index < 0)
            index = _blocks.Length + (index %= _blocks.Length);

        if (index >= _blocks.Length)
            index %= _blocks.Length;
    }

    public void AddCurrentIndex(int offset)
    {
        int index = CurrentIndex + offset;
        FixIndex(ref index);
        CurrentIndex = index;
        UpdateCurrentIndex?.Invoke(index);
    }
}
