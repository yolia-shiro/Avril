using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class Boss : Enemy, IDamageable
{
    protected BehaviorTree behaviorTree;

    [Header("Index")]
    public int bossIndex;
    //private bool isExist = true;   //是否存在

    [Header("Trigger")]
    public bool isTrigger = false;
    public bool haveInit = false;
    public Transform triggerPoint;
    public float triggerRadius;
    public LayerMask triggerLayer;

    [Header("Boundary")]
    public GameObject leftBoundary;
    public GameObject rightBoundary;
    public Transform cameraLockTF;

    protected override void Awake()
    {
        base.Awake();
        behaviorTree = GetComponent<BehaviorTree>();
        behaviorTree.StartWhenEnabled = false;
    }

    public override void Start()
    {
        if (SaveManager.Instance.HaveBossLiveKey && (SaveManager.Instance.BossLive & (1 << bossIndex)) == 0)
        {
            //Boss已经死亡
            //isExist = false;
            Destroy(gameObject);
            return;
        }
        base.Start();
    }

    public virtual void Update() 
    {
        PhysicsCheck();
        if (!isTrigger)
        {
            return;
        }
        if (isDeath)
        {
            behaviorTree.SetVariable("isDeath", (SharedBool)isDeath);
            //TODO: 播放死亡动画

            //注销数据
            GameManager.Instance.LogOutEnemy(this);
            //存储Boss数据
            SaveManager.Instance.SaveBossData();
            Destroy(gameObject, 2.0f);
            //关闭Boss区域的边界p;[l'
            leftBoundary.SetActive(false);
            rightBoundary.SetActive(false);
            //切换相机为跟随状态
            MainCamera.Instance.TranslateToState(MainCamera.CameraState.Follow);
            return;
        }
        if (!haveInit)
        {
            //只触发一次
            //打开Boss区域的边界
            leftBoundary.SetActive(true);
            rightBoundary.SetActive(true);
            //相机锁定视角，不在跟随玩家
            MainCamera.Instance.TranslateToState(MainCamera.CameraState.Lock);
            StartCoroutine(MainCamera.Instance.SetCameraPos(cameraLockTF.position));
            //显示Boss UI
            ui.SetActive(true);
            //UIManager.instance.SetBossUIActive(true, maxHP, maxToughness);
            ////更新Boss UI
            //UIManager.instance.UpdateBossUI(curHP, curToughness);
            //启动行为树
            behaviorTree.EnableBehavior();
            haveInit = true;
        }
    }

    ///// <summary>
    ///// 销毁时，从GameManager中注销，存储当前Boss数据
    ///// </summary>
    //private void OnDisable()
    //{
    //    if (isExist)
    //    {
    //        //存在状态向死亡状态切换时执行
            
    //    }
    //}

    //物理检测
    public void PhysicsCheck()
    {
        if (!isTrigger) 
        {
            isTrigger = Physics2D.OverlapCircle(triggerPoint.position, triggerRadius, triggerLayer);
        }
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(triggerPoint.position, triggerRadius);
    }

    /// <summary>
    /// 接受伤害的接口
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool GetDamage(float value)
    {
        if (!isTrigger)
        {
            //未触发
            return true;
        }
        ResumeHP(value);
        sliderHP.value = curHP;
        ResumeToughness(value * resistance);
        sliderToughness.value = curToughness;
        if (curHP <= 0) 
        {
            //死亡
            isDeath = true;
            //关闭Boss血条UI
            ui.SetActive(false);
            return true;
        }
        if (curToughness <= 0)
        {
            //硬直状态
            isToughness = true;
            return true;
        }
        //TODO: 受击

        return true;
    }

    public override void EndNotify()
    {
        base.EndNotify();
    }

    public override int BossResponse()
    {
        return bossIndex;
    }
}
