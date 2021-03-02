using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class FollowWalkAction : Action
{
    public SharedInt state;
    public float walkSpeed;
    public float followWalkStateMinDuration;
    public float followWalkStateMaxDuration;
    public float limitMinDisToPlayer;

    private PlayerData playerData;
    private PlayerController playerController;
    private PlayerState playerState;
    private float randomWalkStateDuration;
    private float curFollowStateTime;

    public override void OnStart()
    {
        playerData = GameManager.Instance.playerData;
        playerController = playerData.GetComponent<PlayerController>(); 
        playerState = playerData.GetComponent<PlayerState>();

        Owner.SetVariable("state", state);
        curFollowStateTime = 0;
        randomWalkStateDuration = Random.Range(followWalkStateMinDuration, followWalkStateMaxDuration);
    }

    public override TaskStatus OnUpdate()
    {
        if (playerState.isCounterattacked)
        {
            //Owner.SetVariable("");
            return TaskStatus.Failure;
        }

        if (curFollowStateTime < randomWalkStateDuration)
        {
            //朝向，1:向右，-1:向左
            int dir = transform.position.x < playerData.transform.position.x ? 1 : -1;
            transform.rotation = dir == 1 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            //逼近
            //保证y轴不变
            Vector3 followPos = new Vector3(playerData.transform.position.x, transform.position.y, playerData.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, followPos, walkSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, playerData.transform.position) < limitMinDisToPlayer)
            {
                return TaskStatus.Failure;
            }
            curFollowStateTime += Time.deltaTime;
            return TaskStatus.Running;
        }
        return TaskStatus.Failure;
    }

    public override void OnEnd()
    {
        if (playerState.isCounterattacked)
        {
            //100%触发近身攻击
            Owner.SetVariable("canIdle", (SharedBool)false);
            Owner.SetVariable("canSprint", (SharedBool)false);
            Owner.SetVariable("canSlash", (SharedBool)true);
            Owner.SetVariable("canMagicSwordAim", (SharedBool)false);
            Owner.SetVariable("canMagicCannon", (SharedBool)false);
            return;
        }

        Owner.SetVariable("canIdle", (SharedBool)true);
    }
}
