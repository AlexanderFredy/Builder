using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGameButton : MonoBehaviour
{
    [SerializeField] private string _lobbySceneName = "Menu";
    public void OnClick()
    {
        MultiplayerManager.Instance.LeaveGame();
        SceneManager.LoadScene(_lobbySceneName);
    }
}
