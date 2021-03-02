using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IEndGameObserver
{
    [Header("Basic")]
    public float maxHP;         //血量
    protected float curHP;
    public float maxToughness;  //韧性
    protected float curToughness;
    public float CurToughness { get { return curToughness; } set { curToughness = value; } }
    public float resistance;    //抗性

    [Header("Cumulate Property")]
    public float maxBurnCumulate;      //灼伤积累值
    public float curBurnCumulate;      //当前灼伤积累值
    public float maxBurnDuration;   //灼伤持续时间
    public float curBurnDuration;  //当前灼伤持续时间
    public int maxBurnLayerNum;     //灼伤最多叠加层数
    public int curBurnLayerNum;     //当前灼伤叠加层数

    [Header("Lock")]
    public GameObject lockGameObject;

    [Header("State")]
    public bool isDeath;
    public bool isToughness;

    [Header("UI")]
    public GameObject ui;
    public Slider sliderHP;
    public Slider sliderToughness;

    protected Animator anim;
    protected Rigidbody2D myRigidbody;

    //初始信息
    private Vector3 originPosition;
    private Quaternion originRotation;
    protected float originGravityScale;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        curHP = maxHP;
        curToughness = maxToughness;
        //UI
        sliderHP.maxValue = maxHP;
        sliderHP.value = maxHP;
        sliderToughness.maxValue = maxToughness;
        sliderToughness.value = maxToughness;
        if (maxToughness == 0)
        {
            sliderToughness.gameObject.SetActive(false);
        }
        //初始信息
        originPosition = transform.position;
        originRotation = transform.rotation;
        originGravityScale = myRigidbody.gravityScale;

        //FIXME: 场景切换后修改
        GameManager.Instance.RegisterEnemy(this);
    }

    //切换场景时启用
    //private void OnEnable()
    //{
    //    //注册敌人
    //    GameManager.Instance.RegisterEnemy(this);
    //}

    private void OnDisable()
    {
        //注销敌人
        if (!GameManager.IsInitialized)
        {
            return;
        }
        GameManager.Instance.LogOutEnemy(this);
    }

    private void OnBecameVisible()
    {
        if (isDeath)
        {
            return;
        }
        if (GameManager.Instance.playerData != null)
        {
            GameManager.Instance.playerData.GetComponent<PlayerController>().AddLockTarget(transform);
        }
    }
    private void OnBecameInvisible()
    {
        if (GameManager.Instance.playerData != null)
        {
            GameManager.Instance.playerData.GetComponent<PlayerController>().RemoveLockTarget(transform);
        }
    }

    public void SetLockFlagActive(bool active)
    {
        lockGameObject.SetActive(active);
    }

    public void ResumeHP(float value)
    {
        curHP -= value;
        if(curHP < 0) 
        {
            curHP = 0;
        }
    }

    public void ResumeToughness(float value)
    {
        curToughness -= value * (1 - resistance);
        if (curToughness < 0) 
        {
            curToughness = 0;
        }
    }

    /// <summary>
    /// 积累值计算
    /// </summary>
    /// <param name="cumulateValue">积累值</param>
    /// <param name="reduceValuePerSecond">单位灼伤伤害</param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public virtual bool CalCulateCumulateValue(float cumulateValue, float reduceValuePerSecond)
    {
        curBurnCumulate += cumulateValue;
        if (curBurnCumulate >= maxBurnCumulate)
        {
            //灼伤
            StopAllCoroutines();
            curBurnDuration = 0;
            curBurnLayerNum++;
            curBurnLayerNum = curBurnLayerNum >= maxBurnLayerNum ? maxBurnLayerNum : curBurnLayerNum;
            StartCoroutine(ReduceBloodPerSecond(reduceValuePerSecond));
            ////UI
            //UIManager.instance.AddBossDebuff("火", curBurnLayerNum, maxBurnDuration);
            curBurnCumulate = 0;
        }
        return true;
    }

    /// <summary>
    /// 每秒减血
    /// </summary>
    /// <param name="value">扣血量</param>
    /// <returns></returns>
    public IEnumerator ReduceBloodPerSecond(float value)
    {
        //持续n秒，生效n - 1秒
        while (curBurnLayerNum > 0)
        {
            while (curBurnDuration < maxBurnDuration)
            {
                ResumeHP(value * curBurnLayerNum);
                curBurnDuration += 1.0f;
                yield return new WaitForSeconds(1.0f);
            }
            curBurnDuration = 0;
            curBurnLayerNum--;
        }
    }

    public virtual void EndNotify()
    {
        //游戏结束
        //还原信息和位置
        Reset();
    }

    public virtual int BossResponse()
    {
        return -1;
    }

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        curHP = maxHP;
        curToughness = maxToughness;
        //UI
        sliderHP.maxValue = maxHP;
        sliderHP.value = maxHP;
        sliderToughness.maxValue = maxToughness;
        sliderToughness.value = maxToughness;
        if (maxToughness == 0)
        {
            sliderToughness.gameObject.SetActive(false);
        }
        ui.SetActive(false);
        //初始信息
        transform.position = originPosition;
        transform.rotation = originRotation;
        myRigidbody.gravityScale = originGravityScale;
    }
}
