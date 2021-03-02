using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SprintAction : Action
{
    public float cd;
    public SharedInt state;
    public float damage;

    public SharedTransform leftSprintTarget;
    public SharedTransform rightSprintTarget;
    private Vector3 sprintTarget;
    //private bool isPreSprintOver;
    public float sprintWaitTime;    //等待一段时间后释放
    private float curSprintTime;
    public float sprintSpeed;
    //private int sprintNum;  //冲刺次数
    public GameObject sprintEffect;

    private Vector3 playerPosition;
    private GameObject weapon;

    public override void OnStart()
    {
        playerPosition = GameManager.Instance.playerData.transform.position;
        weapon = (Owner.GetVariable("weapon") as SharedGameObject).Value;
        weapon.GetComponent<WizardWeapon>().damage = damage;

        Owner.SetVariable("state", state);
        Owner.SetVariable("isPreSprintOver", (SharedBool)false);
        curSprintTime = 0;
        //获取目标点
        //根据玩家位置确定
        sprintTarget = playerPosition.x > transform.position.x ? rightSprintTarget.Value.position : leftSprintTarget.Value.position;
        transform.rotation = playerPosition.x < transform.position.x ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
    }

    public override TaskStatus OnUpdate()
    {
        if ((Owner.GetVariable("isPreSprintOver") as SharedBool).Value)
        {
            if (curSprintTime < sprintWaitTime)
            {
                curSprintTime += Time.deltaTime;
            }
            else
            {
                weapon.GetComponentInChildren<Collider2D>().enabled = true;
                sprintEffect.SetActive(true);
                transform.position = Vector3.MoveTowards(transform.position, sprintTarget, sprintSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, sprintTarget) < 0.1f)
                {
                    return TaskStatus.Success;
                }
            }
        }
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        Owner.SetVariable("canIdle", (SharedBool)true);
        Owner.SetVariable("canSprint", (SharedBool)false);
        StartCoroutine(Util.instance.SetSharedValueAfterTimeToBT(cd, Owner, "canSprint", (SharedBool)true));

        sprintEffect.SetActive(false);
        weapon.GetComponentInChildren<Collider2D>().enabled = false;
    }
}
