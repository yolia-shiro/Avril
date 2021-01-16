using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistMagic : Magic, IDamageable
{
    //持续时间(对防御) || 释放时间(对治愈)
    public float duration;
    public bool isOver = false;
    [Header("Assist Magic Type")]
    public bool isHeal;
    public bool isDefense;

    [Header("Heal Magic")]
    public float hps;

    [Header("Defense Magic")]
    public float maxDefenseValue;
    private float curDefenseValue;

    private bool isEffect = false;


    public override void Start()
    {
        base.Start();
        curDefenseValue = maxDefenseValue;
    }

    public override void Creating()
    {
        base.Creating();
        StartCoroutine(CheckCreatingOver());
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

    public void GetDamage(float value)
    {
        curDefenseValue -= value;
        if (curDefenseValue <= 0)
        {
            curDefenseValue = 0;
            isOver = true;
        }
    }
}
