using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : NormalEnemy
{
    [Header("Shield")]
    public float minDisShield;  //进行防御的最近极限距离
    public float shieldDuration;
    private float curShieldDuration;
    public float shiledCD;
    protected bool canShiled = true;
    public bool isShielded;    //是否格挡住攻击
    

    //FSM
    public ShieldState shieldState = new ShieldState();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerController>().GetDamage(attackValue[attackRandomIndex]))
            {
                //击中玩家
                Debug.Log($"{name} 击中 {collision.gameObject.name} ----- 伤害为 {attackValue[attackRandomIndex]}");
            }
        }
    }

    public override void Track()
    {
        if (searchTarget == null)
        {
            //目标丢失，切换回巡逻状态
            TranslateToState(patrolState);
            return;
        }
        if (Vector3.Distance(transform.position, searchTarget.position + new Vector3(0, yOffset, 0)) < minDisShield && canShiled)
        {
            //切换到防御状态
            TranslateToState(shieldState);
            return;
        }else if (Vector3.Distance(transform.position, searchTarget.position + new Vector3(0, yOffset, 0)) < minDisToTarget && !canShiled)
        {
            //切换到攻击状态
            anim.SetBool("isPatrol", false);
            TranslateToState(attackState);
            return;
        }
        anim.SetBool("isPatrol", true);
        transform.rotation = transform.position.x < searchTarget.position.x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        transform.position = Vector3.MoveTowards(transform.position, searchTarget.transform.position + new Vector3(0, yOffset, 0), trackSpeed * Time.deltaTime);
    }


    public override void ShieldPreparation()
    {
        anim.SetBool("isPatrol", false);
        anim.SetBool("isShield", true);
        curShieldDuration = 0;
        //转向
        //转向
        if (searchTarget != null)
        {
            transform.rotation = transform.position.x < searchTarget.position.x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        }
    }

    public override void Shield()
    {
        curShieldDuration += Time.deltaTime;
        if (curShieldDuration >= shieldDuration)
        {
            //结束格挡
            anim.SetBool("isShield", false);
            canShiled = false;

            StartCoroutine(WaitShieldCd());
            TranslateToState(trackState);
            return;
        }
    }

    public IEnumerator WaitShieldCd()
    {
        yield return new WaitForSeconds(shiledCD);
        canShiled = true;
    }

    public override bool GetDamage(float value)
    {
        
        ui.SetActive(true);
        if (enemyState is ShieldState && (searchTarget != null && Vector3.Dot(transform.right, searchTarget.transform.right) < 0))
        {
            anim.SetTrigger("shield");
        }
        else 
        {
            curHP -= value;
            sliderHP.value -= value;
            if (enemyState is ShieldState)
            {
                //清除防御状态下的一些数值
                //结束格挡
                anim.SetBool("isShield", false);
                canShiled = false;

                StartCoroutine(WaitShieldCd());
            }
        }
        if (curHP <= 0)
        {
            curHP = 0;
            sliderHP.value = 0;

            isDeath = true;
            anim.SetBool("isDeath", isDeath);
            ui.SetActive(false);
            return true;
        }

        if (!(enemyState is StiffState))
        {
            curToughness -= value * (1 - resistance);
            sliderToughness.value -= value * (1 - resistance);
        }
        if (maxToughness > 0 && curToughness <= 0)
        {
            curToughness = 0;
            sliderToughness.value = 0;

            //进入僵直状态
            if (!(enemyState is StiffState))
            {
                if (enemyState is ShieldState)
                {
                    //清除防御状态下的一些数值
                    //结束格挡
                    anim.SetBool("isShield", false);
                    canShiled = false;

                    StartCoroutine(WaitShieldCd());
                }
                TranslateToState(stiffState);
            }
            return true;
        }
        else if (maxToughness == 0)
        {
            sliderToughness.gameObject.SetActive(false);
        }
        if(!(enemyState is ShieldState) || (searchTarget != null && Vector3.Dot(transform.right, searchTarget.transform.right) >= 0))
        {
            //进入受击状态
            TranslateToState(hitState);
        }
        return true;
    }
}
