using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : Singleton<GameManager>
{
    public PlayerData playerData;   //用户数据

    private List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();     //敌人注册

    protected override void Awake()
    {
        base.Awake();
        //切换场景时不销毁
        DontDestroyOnLoad(this);
        //禁用鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// 观察者模式
    /// 注册用户数据PlayerData
    /// </summary>
    public void RegisterPlayerData(PlayerData playerData) 
    {
        this.playerData = playerData;
    }

    /// <summary>
    /// 观察者模式
    /// 注册敌人数据
    /// </summary>
    /// <param name="enemyObserver"></param>
    public void RegisterEnemy(IEndGameObserver enemyObserver)
    {
        endGameObservers.Add(enemyObserver);
    }

    /// <summary>
    /// 观察者模式
    /// 注销敌人数据
    /// </summary>
    /// <param name="enemyObserver"></param>
    public void LogOutEnemy(IEndGameObserver enemyObserver)
    {
        if (endGameObservers.Contains(enemyObserver))
        {
            endGameObservers.Remove(enemyObserver);
        }
    }

    /// <summary>
    /// 执行广播接口
    /// </summary>
    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    /// <summary>
    /// 从所有注册的对象中，获取所有Boss的存活状态
    /// </summary>
    /// <returns></returns>
    public int GetAllBossLiveState()
    {
        int res = 0;
        foreach (var observer in endGameObservers)
        {
            int index;
            if ((index = observer.BossResponse()) != -1)
            {
                res |= (1 << index);
            }
        }
        return res;
    }

    //public Save CreateSaveGameObject()
    //{
    //    Save save = new Save();
    //    save.playerPositionX = player.transform.position.x;
    //    save.playerPositionY = player.transform.position.y;
    //    save.cameraPositionX = Camera.main.transform.position.x;
    //    save.cameraPositionY = Camera.main.transform.position.y;
    //    save.sceneIndex = SceneManager.GetActiveScene().buildIndex;

    //    return save;
    //}

    ////Object To JSON
    //public void SaveByJSON()
    //{
    //    Save save = CreateSaveGameObject();

    //    string jsonString = JsonUtility.ToJson(save);
    //    StreamWriter sw = new StreamWriter(Application.dataPath + "/JSONData.txt");
    //    sw.Write(jsonString);
    //    sw.Close();     //关闭流
    //    Debug.Log("========= SAVE =========");
    //}

    ////JSON To Object
    //public bool LoadByJSON()
    //{
    //    if (!File.Exists(Application.dataPath + "/JSONData.txt"))
    //    {
    //        Debug.Log("文件不存在");
    //        return false;
    //    }
    //    StreamReader sr = new StreamReader(Application.dataPath + "/JSONData.txt");
    //    string jsonString = sr.ReadToEnd();
    //    Save save = JsonUtility.FromJson<Save>(jsonString);

    //    player.transform.position = new Vector2(save.playerPositionX, save.playerPositionY);
    //    Camera.main.transform.position = new Vector3(save.cameraPositionX, save.cameraPositionY, Camera.main.transform.position.z);
    //    wantLoadSceneIndex = save.sceneIndex;

    //    Debug.Log("======== LOAD =========");
    //    return true;
    //}

    ///// <summary>
    ///// 异步加载场景（含进度条）
    ///// </summary>
    ///// <returns></returns>
    //public IEnumerator AsynchronousLoadScene(int sceneIndex)
    //{
    //    //打开加载UI
    //    int displayProgress = 0;
    //    int toProgress = 0;
    //    UIManager.instance.SetLoadInterfaceActive(true);
    //    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
    //    operation.allowSceneActivation = false;
    //    while (operation.progress < 0.9f) 
    //    {
    //        toProgress = (int)(operation.progress * 100);
    //        while (displayProgress < toProgress)
    //        {
    //            displayProgress++;
    //            //更新UI
    //            UIManager.instance.UpdateLoadSlider(displayProgress);
    //            yield return new WaitForEndOfFrame();
    //        }
    //    }
    //    toProgress = 100;
    //    while (displayProgress < toProgress) 
    //    {
    //        displayProgress++;
    //        UIManager.instance.UpdateLoadSlider(displayProgress);
    //        yield return new WaitForEndOfFrame();
    //    }
    //    operation.allowSceneActivation = true;
    //}

    ///// <summary>
    ///// 异步开始
    ///// </summary>
    //public void AsynchronousStartGame()
    //{
    //    PlayerPrefs.DeleteAll();
    //    StartCoroutine(AsynchronousLoadScene(SceneManager.GetActiveScene().buildIndex + 1)); 
    //}

    ///// <summary>
    ///// 异步继续
    ///// </summary>
    //public void AsynchronousContinue()
    //{
    //    //LoadByJSON();
    //    //StartCoroutine(AsynchronousLoadScene(wantLoadSceneIndex));
    //    //LoadByJSON();
    //}

    ///// <summary>
    ///// 重新开始
    ///// </summary>
    //public void Reset()
    //{
    //    //LoadByJSON();
    //    //StartCoroutine(AsynchronousLoadScene(wantLoadSceneIndex));
    //    //LoadByJSON();
    //}

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameOver()
    {
        //退出游戏
        Application.Quit();
    }

    public Transform GetNewGameOriginPosition()
    {
        return GameObject.FindGameObjectWithTag("New Game Position").transform;
    }
}
