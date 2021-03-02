using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    public GameObject playerPrefabs;
    public GameObject mainCameraPrefabs;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// 加载新游戏
    /// </summary>
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadNewGame("Scene01"));
    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    IEnumerator LoadNewGame(string sceneName)
    {
        if (sceneName != "") 
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            Vector3 pos = GameManager.Instance.GetNewGameOriginPosition().position;
            yield return Instantiate(playerPrefabs,
                new Vector3(pos.x, pos.y, 0),
                Quaternion.identity);
            //新游戏开始后进行一次存档
            //存储玩家信息
            SaveManager.Instance.SavePlayeraData();
            //存储所有Boss信息
            SaveManager.Instance.SaveBossData();
        }  
    }

    IEnumerator LoadLevel(string sceneName)
    {
        if (sceneName != "")
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefabs, 
                new Vector3(SaveManager.Instance.PlayerPositionX, SaveManager.Instance.PlayerPositionY, 0),
                Quaternion.identity);
        }
    }
}
