using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SprintConditional : Conditional
{
    private Vector3 playerPos;
    private Vector3 selfPos;
    private bool canSprint;
    public float limitDisToPlayer;

    public override void OnStart()
    {
        canSprint = (Owner.GetVariable("canSprint") as SharedBool).Value;
        playerPos = GameManager.Instance.playerData.transform.position;
        selfPos = transform.position;
    }

    public override TaskStatus OnUpdate()
    {
        if (!canSprint || Vector3.Distance(selfPos, playerPos) < limitDisToPlayer)
        {
            return TaskStatus.Failure;
        }
        return TaskStatus.Success;
    }
}
