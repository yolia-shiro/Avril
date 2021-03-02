using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<MagicType, string> magicTypeToStr = new Dictionary<MagicType, string>();

    [Header("Opening Menu")]
    public GameObject openingMenu;
    public Button continueButton;
    public GameObject openingMenuFirstButton;

    [Header("Player UI")]
    public Slider HPSlider;
    public Slider MPSlider;
    public Slider BPSlider;

    public GameObject lockModeUI;

    [Header("Storage Magic")]
    public List<Sprite> storageMagicUIImg;
    public Transform storageContainer;
    public GameObject storageMagicUIPrefabs;
    private List<GameObject> curStorageMagicUI = new List<GameObject>();

    [Header("Resonance")]
    public Text resonanceText;
    public Slider resonanceSlider;

    //[Header("Boss")]
    //public GameObject bossUI;
    //public Slider bossHpSlider;
    //public Slider bossToughness;

    //[Header("Boss Debuff")]
    //public GameObject bossDebuffs;  //Debuff列表
    //public Dictionary<string, GameObject> bossDebuffDict = new Dictionary<string, GameObject>();
    //public GameObject bossDebuff;   //Debuff预制体

    [Header("Switch Resonance Menu")]
    public GameObject switchResonanceMenu;
    public float openResonanceMenuTimeScale;
    public float closeResonanceMenuTimeScale;

    [Header("Save Position Menu")]
    public GameObject saveMenu;
    public GameObject saveMenuFirstButton;

    [Header("Load Interface")]
    public GameObject loadInterface;
    public Slider loadSlider;
    public Text loadText;

    // Start is called before the first frame update
    void Start()
    {
        if (openingMenu != null)
        {
            //当前处于开始菜单
            EventSystem.current.SetSelectedGameObject(openingMenuFirstButton);
            //continueButton.interactable = File.Exists(Application.dataPath + "/JSONData.txt");
            continueButton.interactable = SaveManager.Instance.HaveSaveMessage;
        }
        else
        {
            magicTypeToStr.Add(MagicType.Nihility, "无");
            magicTypeToStr.Add(MagicType.Fire, "火");
            magicTypeToStr.Add(MagicType.Water, "水");
            magicTypeToStr.Add(MagicType.Wood, "木");

            HPSlider.maxValue = GameManager.Instance.playerData.MaxHP;
            MPSlider.maxValue = GameManager.Instance.playerData.MaxMP;
            BPSlider.maxValue = GameManager.Instance.playerData.MaxBP;

            resonanceSlider.maxValue = GameManager.Instance.playerData.MaxResonanceValue;
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if (openingMenu != null)
        {
            //开始界面
            return;
        }

        HPSlider.value = GameManager.Instance.playerData.CurHP;
        MPSlider.value = GameManager.Instance.playerData.CurMP;
        BPSlider.value = GameManager.Instance.playerData.CurBP;

        PlayerController playerController = GameManager.Instance.playerData.GetComponent<PlayerController>();
        //Resonance
        UpdateResonance((MagicType)GameManager.Instance.playerData.CurResonanceType, GameManager.Instance.playerData.CurResonanceValue);

        if (saveMenu.activeInHierarchy && Input.GetButtonDown("Cancel"))
        {
            SetSaveMenuActive(false);
            playerController.TranslateToState(playerController.playerMoveState);
        }
    }

    public void SetLockModeActive(bool active)
    {
        lockModeUI.SetActive(active);
    }


    public void UpdateResonance(MagicType magicType, float value)
    {
        resonanceText.text = magicTypeToStr[magicType];
        resonanceSlider.value = value;
    }

    #region Debuff
    //public void AddBossDebuff(string type, int layerNum, float duration)
    //{
    //    GameObject debuff = null;
    //    if (bossDebuffDict.ContainsKey(type))
    //    {
    //        debuff = bossDebuffDict[type];
    //    }
    //    else 
    //    {
    //        debuff = Instantiate(bossDebuff, bossDebuffs.transform);
    //        bossDebuffDict.Add(type, debuff);
    //    }
    //    debuff.GetComponent<Debuff>().SetAttr(type, layerNum, duration);
    //    debuff.GetComponent<Debuff>().Restart();
    //}

    //public void RemoveBossDebuff(string key)
    //{
    //    bossDebuffDict.Remove(key);
    //}
    #endregion

    #region 共鸣界面
    /// <summary>
    /// 切换共鸣元素界面
    /// </summary>
    /// <param name="active"></param>
    public void SwitchResonanceMenu(bool active)
    {
        if (active)
        {
            Time.timeScale = openResonanceMenuTimeScale;
        } 
        else
        {
            Time.timeScale = closeResonanceMenuTimeScale;
        }
        switchResonanceMenu.SetActive(active);
    }

    /// <summary>
    /// 获取选择的共鸣元素类型
    /// </summary>
    /// +1 的原因：MagicType中有无属性魔法在0位
    /// <returns></returns>
    public MagicType ReturnSelectedResonanceType()
    {
        return (MagicType)(switchResonanceMenu.GetComponent<SwitchResonanceMenu>().GetSelectedIndex() + 1);
    }
    #endregion

    #region 休息（存储）菜单
    /// <summary>
    /// 设置存储点处出现菜单的显隐
    /// </summary>
    /// <param name="active"></param>
    public void SetSaveMenuActive(bool active)
    {
        saveMenu.SetActive(active);
        if (active)
        {
            //设置聚焦点
            EventSystem.current.SetSelectedGameObject(saveMenuFirstButton);
        }
        else
        {
            PlayerController playerController = GameManager.Instance.playerData.GetComponent<PlayerController>();
            playerController.TranslateToState(playerController.playerMoveState);
        }
    }
    #endregion

    #region 加载界面
    /// <summary>
    /// 设置加载界面的显隐
    /// </summary>
    /// <param name="active"></param>
    public void SetLoadInterfaceActive(bool active)
    {
        if (loadInterface != null)
        {
            loadInterface.SetActive(active);
        }
    }

    /// <summary>
    /// 更新加载进度条信息（百分制）
    /// </summary>
    public void UpdateLoadSlider(float value)
    {
        if (loadSlider != null && loadText != null)
        {
            loadSlider.value = value;
            loadText.text = loadSlider.value + "%";
        }
        else
        {
            Debug.LogError("加载界面缺少物体");
        }
    }
    #endregion

    #region Boss UI
    ///// <summary>
    ///// 设置BossUI的显隐
    ///// </summary>
    ///// <param name="active">显隐值</param>
    //public void SetBossUIActive(bool active, float maxHP = 0.0f, float maxToughness = 0.0f) 
    //{
    //    if(bossUI.activeInHierarchy != active)
    //    {
    //        bossUI.SetActive(active);
    //        if (active)
    //        {
    //            //设置Boss UI的初始值
    //            bossHpSlider.maxValue = maxHP;
    //            bossToughness.maxValue = maxToughness;
    //        }
    //    }
    //}
    ///// <summary>
    ///// 更新Boss UI信息
    ///// </summary>
    ///// <param name="hp"></param>
    ///// <param name="toughness"></param>
    //public void UpdateBossUI(float hp, float toughness)
    //{
    //    bossHpSlider.value = hp;
    //    bossToughness.value = toughness;
    //}
    #endregion

    public void NewGame()
    {
        //清除保存数据
        PlayerPrefs.DeleteAll();
        //加载场景
        SceneController.Instance.TransitionToFirstLevel();
    }

    public void Continue()
    {
        SceneController.Instance.TransitionToLoadGame();
    }
}
