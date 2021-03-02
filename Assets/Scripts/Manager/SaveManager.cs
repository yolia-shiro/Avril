using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    private readonly string saveKey = "save";
    private readonly string sceneKey = "level";
    private readonly string playerSavePositionXKey = "playerPosition_X";
    private readonly string playerSavePositionYKey = "playerPosition_Y";
    private readonly string cameraSavePositionXKey = "cameraPosition_X";
    private readonly string cameraSavePositionYKey = "cameraPosition_Y";
    private readonly string bossLiveStateKey = "bossLive";

    public bool HaveSaveMessage { get { return PlayerPrefs.HasKey(saveKey); } }     //是否有存档数据
    public string SceneName { get { return PlayerPrefs.GetString(sceneKey); } }
    public float PlayerPositionX { get { return PlayerPrefs.GetFloat(playerSavePositionXKey); } }
    public float PlayerPositionY { get { return PlayerPrefs.GetFloat(playerSavePositionYKey); } }
    public float CameraPositionX { get { return PlayerPrefs.GetFloat(cameraSavePositionXKey); } }
    public float CameraPositionY { get { return PlayerPrefs.GetFloat(cameraSavePositionYKey); } }
    public bool HaveBossLiveKey{ get { return PlayerPrefs.HasKey(bossLiveStateKey); } }
    public int BossLive { get { return PlayerPrefs.GetInt(bossLiveStateKey); } }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Delete PlayerPrefs"))
        {
            PlayerPrefs.DeleteAll();
        }
        if (GUILayout.Button("Load"))
        {
            //LoadPlayerData();
            SceneController.Instance.TransitionToLoadGame();
        }
    }

    public void SavePlayeraData()
    {
        Save(GameManager.Instance.playerData.playerStatus_SO, GameManager.Instance.playerData.playerStatus_SO.name);
        Debug.Log("======== SAVE Player Data ========");
    }

    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerData.playerStatus_SO, GameManager.Instance.playerData.playerStatus_SO.name);
        Debug.Log("======== LOAD Player Data ========");
    }

    /// <summary>
    /// 保存Boss的数据
    /// </summary>
    public void SaveBossData()
    {
        PlayerPrefs.SetInt(bossLiveStateKey, GameManager.Instance.GetAllBossLiveState());
        PlayerPrefs.Save();
        Debug.Log("======== SAVE Boss Data ========");
    }

    public void Save(Object data, string key)
    {
        string jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(saveKey, "Save Flag");
        PlayerPrefs.SetString(sceneKey, SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat(playerSavePositionXKey, GameManager.Instance.playerData.transform.position.x);
        PlayerPrefs.SetFloat(playerSavePositionYKey, GameManager.Instance.playerData.transform.position.y);
        PlayerPrefs.SetFloat(cameraSavePositionXKey, Camera.main.transform.position.x);
        PlayerPrefs.SetFloat(cameraSavePositionYKey, Camera.main.transform.position.y);
        PlayerPrefs.Save();
    }

    public void Load(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }
}
