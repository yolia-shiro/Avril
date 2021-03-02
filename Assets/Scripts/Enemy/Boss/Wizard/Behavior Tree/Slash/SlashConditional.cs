using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SlashConditional : Conditional
{
    private bool canSlash;
    public float limitMaxDisToPlay;

    private PlayerController playerController;
    private Vector3 playerPos;
    private Vector3 selfPos;

    public override void OnStart()
    {
        canSlash = (Owner.GetVariable("canSlash") as SharedBool).Value;
        playerPos = GameManager.Instance.playerData.transform.position;
        playerController = GameManager.Instance.playerData.GetComponent<PlayerController>();
        selfPos = transform.position;
    }

    public override TaskStatus OnUpdate()
    {
        if (!canSlash || Vector3.Distance(playerPos, selfPos) > limitMaxDisToPlay || !playerController.isAttackEffective)
        {
            return TaskStatus.Failure;
        }
        return TaskStatus.Success;
    }
}
