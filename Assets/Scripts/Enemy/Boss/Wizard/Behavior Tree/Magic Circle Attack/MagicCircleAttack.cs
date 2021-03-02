using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class MagicCircleAttack : Action
{
    private Rigidbody2D selfRigidbody;

    public float cd;
    public SharedInt state;
    public SharedTransform releasePos;
    public float moveSpeed;

    [Header("Circle Attack")]
    public Transform point;
    public float radius;
    public int layerNum;      //层数
    private int curCreateLayerNum = 0;
    public float perAngle;      //每个魔法之间的角度
    public float waitTime;      //每层之间释放的等待数
    private float curWaitTime;

    public GameObject magicPrefabs;

    private bool isMoveToTarget;
    public override void OnStart()
    {
        selfRigidbody = GetComponent<Rigidbody2D>();

        Owner.SetVariable("state", state);
        ////关闭重力
        selfRigidbody.bodyType = RigidbodyType2D.Kinematic;
        selfRigidbody.velocity = Vector2.zero;
        isMoveToTarget = false;

        curWaitTime = waitTime;
        curCreateLayerNum = 0;
    }

    public override TaskStatus OnUpdate()
    {
        if (Vector2.Distance(transform.position, releasePos.Value.position) >= 0.1f && !isMoveToTarget)
        {
            //移动到指定位置
            Util.instance.MoveToTargetPos(transform, releasePos.Value.position, false, moveSpeed);
            return TaskStatus.Running;
        }
        else
        {
            isMoveToTarget = true;
            //释放技能
            if (curCreateLayerNum >= layerNum)
            {
                //释放结束
                //打开重力
                selfRigidbody.bodyType = RigidbodyType2D.Dynamic;
                if ((Owner.GetVariable("isGround") as SharedBool).Value)
                {
                    //结束当前状态
                    selfRigidbody.bodyType = RigidbodyType2D.Dynamic;
                    return TaskStatus.Success;
                }
                else
                {
                    return TaskStatus.Running;
                }
            }
            if (curWaitTime < waitTime)
            {
                curWaitTime += Time.deltaTime;
            }
            else
            {
                curWaitTime = 0;
                CreateMagicsInLayer();
                curCreateLayerNum++;
            }
            return TaskStatus.Running;
        }
    }

    public override void OnEnd()
    {
        Owner.SetVariable("canIdle", (SharedBool)true);
        Owner.SetVariable("canMagicCircleAttack", (SharedBool)false);
        StartCoroutine(Util.instance.SetSharedValueAfterTimeToBT(cd, Owner, "canMagicCircleAttack", (SharedBool)true));
    }

    void CreateMagicsInLayer()
    {
        for (float curAngle = 0; curAngle <= 360; curAngle += perAngle)
        {
            float randomAngleOffset = Random.Range(0, 360.0f);
            float x = radius * Mathf.Cos((curAngle + randomAngleOffset) * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(curAngle + randomAngleOffset * Mathf.Deg2Rad);
            GameObject go = Object.Instantiate(magicPrefabs, transform);
            go.transform.localPosition = new Vector3(x, y, 0);
            go.transform.right = go.transform.position - transform.position;
            go.transform.SetParent(null);
        }
    }

    public override void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(point.position, radius);
    }
}
