using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IdleConditional : Conditional
{
    private bool canIdle;

    public override void OnStart()
    {
        canIdle = (Owner.GetVariable("canIdle") as SharedBool).Value;
    }

    public override TaskStatus OnUpdate()
    {
        return canIdle ? TaskStatus.Success : TaskStatus.Failure;
    }
}
