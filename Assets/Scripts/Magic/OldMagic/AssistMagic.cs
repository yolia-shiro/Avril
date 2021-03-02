using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistMagic : Magic, IDamageable
{
    [Header("Assist Magic Message")]
    public float duration;      //持续时间(对防御) || 释放时间(对治愈)
    [HideInInspector] public bool isOver = false;

    [Header("Assist Magic Type")]
    public bool isHeal;
    public bool isDefense;

    [Header("Heal Magic")]
    public float hps;

    [Header("Defense Magic")]
    public float maxDefenseValue;
    private float curDefenseValue;
    private float defenseStartTime; //用于判断是否完美防御
    public float defenseStartTimeOffset; //完美防御的偏差值

    private bool isEffect = false;

    public override void Start()
    {
        base.Start();
        curDefenseValue = maxDefenseValue;
        defenseStartTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDefense)
        {
            return;
        }
        if (collision.CompareTag("Enemy Attack"))
        {
            if (Mathf.Abs(Time.time - defenseStartTime) < defenseStartTimeOffset)
            {
                Debug.Log("完美防御，恢复魔力");
                //时间变缓
                //脚本如果不存在，将会停止当前脚本开启的协程
                Util.instance.StartSlowTime(0.5f, 0.3f);
                StartCoroutine(MainCamera.Instance.Shake(0.1f, 0.08f));
            }
            Debug.Log("防御成功，减少当前防御值 --- damage");
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1.0f;
    }
    public override void Creating()
    {
        base.Creating();
        StartCoroutine(CheckCreatingOver());
    }

    public override bool CheckCreateOver()
    {
        return isOver;
    }

    public override void Effective()
    {
        
        if (isHeal)
        {
            HealOver();
        }
        if (isDefense)
        {
            DefenseOver();
        }    
    }

    public IEnumerator CheckCreatingOver()
    {
        yield return new WaitForSeconds(duration);
        if (this != null)
        {
            isOver = true;
        }
    }

    public void HealOver()
    {
        if (isEffect) 
        {
            return;
        }
        isEffect = true;
        Debug.Log("Heal Magic ----- " + hps);
        CreateHitEffect(transform.position, Quaternion.identity);
    }

    public void DefenseOver() 
    {
        StartCoroutine(MissileToTargetScale(Vector3.zero));
        if (Vector3.Distance(transform.localScale, Vector3.zero) < 0.1)
        {
            Destroy(this.gameObject);
        }
    }

    public bool GetDamage(float value)
    {
        curDefenseValue -= value;
        if (curDefenseValue <= 0)
        {
            curDefenseValue = 0;
            isOver = true;
        }
        return true;
    }
}
