using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockMap : MonoBehaviour
{
    private const string BLOCK_SPAWN_MESSAGE = "BlockSpawn";
    private const string BLOCK_DESTROY_MESSAGE = "BlockDestroy";
    [SerializeField] private BlockManager _manager;
    [SerializeField] private BlockAim _aim;
    private Dictionary<Vector3Int, Block> _map = new();

    private void Start()
    {
        MultiplayerManager.Instance.ApplyBlockSpawn += SpawnBlockFromJSON;
        MultiplayerManager.Instance.ApplyBlockDestroy += DestroyBlock;
    }

    public void SpawnBlock()
    {
        if (TrySpawnBlock(_manager.CurrentIndex, _aim.Position, _aim.Rotation) == false) return;

        string json = JsonUtility.ToJson(new Wrapper(_manager.CurrentIndex, _aim.Position, _aim.Rotation));
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"key", JsonUtility.ToJson(new Vector3IntWrapper(_aim.Position))},
            {"json", json }
        };
        
        MultiplayerManager.Instance.SendMessage(BLOCK_SPAWN_MESSAGE, data);
    }

    public void SpawnBlockFromJSON(string jsonInfo)
    {
        Wrapper wrapper = JsonUtility.FromJson<Wrapper>(jsonInfo);
        TrySpawnBlock(wrapper.currentIndex, wrapper.position, wrapper.rotation);
    }

    public bool TrySpawnBlock(int currentIndex, Vector3Int position, Quaternion rotation)
    {
        if (_map.ContainsKey(position)) return false;
        
        Block block = Instantiate(_manager.GetBlock(currentIndex), position, rotation);
        _map.Add(position, block);

        return true;
    }

    public void DestroyBlock(in Vector3 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo) == false) return;
        if (hitInfo.collider.gameObject.TryGetComponent(out Block block) == false) return;

        Vector3Int key = _map.FirstOrDefault(x => x.Value == block).Key;
        if (_map.ContainsKey(key))
        {
            _map.Remove(key);
            MultiplayerManager.Instance.SendMessage(BLOCK_DESTROY_MESSAGE,JsonUtility.ToJson(new Vector3IntWrapper(key)));
        }

        block.Destroy();
    }

    private void DestroyBlock(string jsonKey)
    {
        Vector3IntWrapper keyWrapper = JsonUtility.FromJson<Vector3IntWrapper>(jsonKey);
        Vector3Int key = keyWrapper.ToVector3Int();

        if (_map.ContainsKey(key) == false) return;

        _map[key].Destroy();
        _map.Remove(key);
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.ApplyBlockSpawn -= SpawnBlockFromJSON;
        MultiplayerManager.Instance.ApplyBlockDestroy -= DestroyBlock;
    }

    [System.Serializable]
    public struct Wrapper
    {
        public Wrapper(int currentIndex, Vector3Int position, Quaternion rotation)
        {
            this.currentIndex = currentIndex;
            this.position = position;
            this.rotation = rotation;
        }

        public int currentIndex;
        public Vector3Int position;
        public Quaternion rotation;
    }

    [System.Serializable]
    public struct Vector3IntWrapper
    {
        public Vector3IntWrapper(Vector3Int value)
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }

        public int x;
        public int y;
        public int z;

        public Vector3Int ToVector3Int()
        {
            return new Vector3Int(x, y, z);
        }
    }

}
