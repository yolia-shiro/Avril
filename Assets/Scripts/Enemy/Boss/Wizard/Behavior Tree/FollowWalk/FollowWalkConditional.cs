using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class FollowWalkConditional : Conditional
{
    private bool canFollowWalk;

    public override void OnStart()
    {
        canFollowWalk = (Owner.GetVariable("canFollowWalk") as SharedBool).Value;
    }

    public override TaskStatus OnUpdate()
    {
        return canFollowWalk ? TaskStatus.Success : TaskStatus.Failure;
    }
}
