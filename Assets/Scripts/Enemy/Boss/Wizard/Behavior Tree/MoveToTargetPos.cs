using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class MoveToTargetPos : Action
{
    public SharedGameObject target;
    public Vector3 offset;
    public bool blink;
    public float speed;
    public bool ignoreYAxis;

    private Vector3 targetPos;

    public override void OnStart()
    {
        if (target == null) return;
        //targetPos = target.Value.transform.position + new Vector3(dir * offset.x, offset.y, offset.z);
        //if (ignoreYAxis)
        //{
        //    //保证y轴不变
        //    targetPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
        //}
    }

    public override TaskStatus OnUpdate()
    {
        if (target == null)
        {
            return TaskStatus.Failure;
        }
        //实时更新位置
        int dir = target.Value.transform.right.x < 0 ? 1 : -1;
        targetPos = target.Value.transform.position + new Vector3(dir * offset.x, offset.y, offset.z);
        if (ignoreYAxis)
        {
            //保证y轴不变
            targetPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
        }
        if (blink)
        {
            transform.position = targetPos;
            return TaskStatus.Success;
        }
        else
        {
            transform.rotation = transform.position.x < targetPos.x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }
}
