using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public PlayerData playerData;
    private MagicSystem magicSystem;

    public Slider HPSlider;
    public Slider MPSlider;
    public Slider BPSlider;

    public Image magicAttach;

    public Transform attackMagicKind;
    public Transform assistMagicKind;

    [Header("Storage Magic")]
    public List<Sprite> storageMagicUIImg;
    public Transform storageContainer;
    public GameObject storageMagicUIPrefabs;
    private List<GameObject> curStorageMagicUI = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        HPSlider.maxValue = playerData.maxHP;
        MPSlider.maxValue = playerData.maxMP;
        BPSlider.maxValue = playerData.maxBP;

        magicSystem = playerData.GetComponent<MagicSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        HPSlider.value = playerData.curHP;
        MPSlider.value = playerData.curMP;
        BPSlider.value = playerData.curBP;
        magicAttach.fillAmount = playerData.curMagicAttachValue;
    }

    //更新攻击魔法的种类
    public void UpdateAttackMagicKind(int attackKind)
    {
        for (int i = 0; i < attackMagicKind.childCount; i++)
        {
            attackMagicKind.GetChild(i).gameObject.SetActive(i == Mathf.Log(attackKind, 2));
        }
    }
    //更新辅助魔法的种类
    public void UpdateAssistMagicKind(int assistKind)
    {
        for (int i = 0; i < assistMagicKind.childCount; i++)
        {
            assistMagicKind.GetChild(i).gameObject.SetActive(i == assistKind);
        }
    }

    //更新存储魔法的UI
    public void UpdateStorageMagicUI(AttackMagic storageMagic) 
    {
        if (storageMagic == null)
        {
            return;
        }
        GameObject storageMagicUIObj = Instantiate(storageMagicUIPrefabs, storageContainer);
        curStorageMagicUI.Add(storageMagicUIObj);
        storageMagicUIObj.GetComponent<ImageFillAmountToZero>().TimeStart(storageMagicUIImg[storageMagic.GetMagicType()], magicSystem.storageTime);
    }

    //移除存储魔法UI
    public void RemoveStorageMagicUI(int removeIndex)
    {
        //储存魔法发生变化时，更新UI列表
        Destroy(curStorageMagicUI[removeIndex]);
        curStorageMagicUI.RemoveAt(removeIndex);
    }
}
