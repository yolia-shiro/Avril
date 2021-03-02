using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class MagicSwordAimConditional : Conditional
{
    private bool canMagicSwordAim;

    public override void OnStart()
    {
        canMagicSwordAim = (Owner.GetVariable("canMagicSwordAim") as SharedBool).Value;
    }

    public override TaskStatus OnUpdate()
    {
        if (!canMagicSwordAim)
        {
            return TaskStatus.Failure;
        }
        return TaskStatus.Success;
    }
}
