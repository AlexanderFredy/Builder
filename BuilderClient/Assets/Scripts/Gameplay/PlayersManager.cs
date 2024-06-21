using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private EnemyController _enemyPrefab;
    private MultiplayerManager _multiplayerManager;
    private Dictionary<string, EnemyController> _enemies = new();

    void Start()
    {
        _multiplayerManager = MultiplayerManager.Instance;
        _multiplayerManager.OnAddPlayer += OnAddplayer;
        _multiplayerManager.OnRemovePlayer += RemoveEnemy;
    }

    void OnDestroy()
    {
        _multiplayerManager.OnAddPlayer -= OnAddplayer;
        _multiplayerManager.OnRemovePlayer -= RemoveEnemy;
    }

    private void OnAddplayer(string key, Player player)
    {
        if (key == MultiplayerManager.Instance.SessionID) CreatePlayer(player);
        else CreateEnemy(key, player);
    }

    private void CreatePlayer(Player player)
    {
        var position = new Vector3(player.pX, player.pY, player.pZ);

        Quaternion rotation = Quaternion.Euler(0, player.rY, 0);
        Instantiate(_playerPrefab, _spawnPoint.position, rotation);
        //player.OnChange += playerCharacter.OnChange;
    }

    private void CreateEnemy(string key, Player player)
    {
        var position = new Vector3(player.pX, player.pY, player.pZ);

        var enemy = Instantiate(_enemyPrefab, _spawnPoint.position, Quaternion.identity);
        enemy.Init(key, player);

        _enemies.Add(key, enemy);
    }

    private void RemoveEnemy(string key, Player player)
    {
        if (_enemies.ContainsKey(key) == false) return;

        var enemy = _enemies[key];
        enemy.Destroy();

        _enemies.Remove(key);
    }
}