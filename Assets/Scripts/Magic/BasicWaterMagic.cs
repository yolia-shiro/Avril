using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWaterMagic : BasicMagic
{
    [Header("Exclusive")]
    public bool canTrack;
    public float rotateSpeed;   //追踪过程中旋转方向的速度

    private Transform aimTarget;    //追踪对象

    public override void Start()
    {
        if (canTrack)
        { 
            transform.right = (aimTarget.position - transform.position).normalized;
        }
    }

    public override void Update()
    {
        if (canTrack)
        {
            //追踪
            if (aimTarget == null)
            {
                return;
            }
            Vector3 dir = (aimTarget.position - transform.position).normalized;
            transform.right = Vector3.MoveTowards(transform.right, dir, rotateSpeed * Time.deltaTime);
        }
        //发射
        transform.position = transform.position + (transform.right + offset) * speed * Time.deltaTime;
    }

    public void SetAimTarget(Transform target)
    {
        aimTarget = target;   
    }

}
