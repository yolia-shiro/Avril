using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class MagicCannonConditional : Conditional
{
    private bool canMagicCannon;
    private GameObject player;

    public SharedTransform cannonLeftReleasePos;      //释放魔炮的左边界位置
    public SharedTransform cannonRightReleasePos;     //释放魔炮的右边界位置
    private Transform curCannonReleasePos;
    public float limitMinDis;   //玩家距离魔炮释放位置的最近距离, 小于该距离不会触发魔炮

    public override void OnStart()
    {
        player = (Owner.GetVariable("player") as SharedGameObject).Value;
        canMagicCannon = (Owner.GetVariable("canMagicCannon") as SharedBool).Value;
        //确定释放位置
        //向着远离Player的方向
        curCannonReleasePos = transform.position.x < player.transform.position.x ? cannonLeftReleasePos.Value : cannonRightReleasePos.Value;
    }

    public override TaskStatus OnUpdate()
    {
        if (!canMagicCannon || Vector3.Distance(player.transform.position, transform.position) < limitMinDis)
        {
            return TaskStatus.Failure;
        }
        return TaskStatus.Success;
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
