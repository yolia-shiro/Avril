using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class IdleAction : Action
{
    public SharedInt state;
    public override TaskStatus OnUpdate()
    {
        Owner.SetVariable("state", state);
        return TaskStatus.Success;
    }

    public override void OnEnd()
    {
        Owner.SetVariable("canIdle", (SharedBool)false);

        //Owner.SetVariable("canSprint", (SharedBool)true);
        //Owner.SetVariable("canSlash", (SharedBool)true);
        //Owner.SetVariable("canMagicSwordAim", (SharedBool)true);
        //Owner.SetVariable("canMagicCannon", (SharedBool)true);
        //Owner.SetVariable("canThrowWeapon", (SharedBool)true);
        //Owner.SetVariable("canMagicCircleAttack", (SharedBool)true);
    }
}
