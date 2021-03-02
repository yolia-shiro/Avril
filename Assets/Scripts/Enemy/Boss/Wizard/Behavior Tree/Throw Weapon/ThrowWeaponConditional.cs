using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class ThrowWeaponConditional : Conditional
{
    private GameObject player;
    private bool canThrowWeapon;

    public float limitMinDis;   //释放的最小距离

    public override void OnStart()
    {
        player = (Owner.GetVariable("player") as SharedGameObject).Value;
        canThrowWeapon = (Owner.GetVariable("canThrowWeapon") as SharedBool).Value;
    }

    public override TaskStatus OnUpdate()
    {
        if (!canThrowWeapon || Vector3.Distance(transform.position, player.transform.position) < limitMinDis)
        {
            return TaskStatus.Failure;
        }
        return TaskStatus.Success;
    }
}
