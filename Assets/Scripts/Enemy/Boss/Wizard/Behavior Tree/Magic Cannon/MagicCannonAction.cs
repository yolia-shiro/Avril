using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class MagicCannonAction : Action
{
    private GameObject player;
    private Rigidbody2D selfRigidbody;

    public float cd;
    public SharedInt state;
    public float damage;
    public SharedTransform cannonLeftReleasePos;      //释放魔炮的左边界位置
    public SharedTransform cannonRightReleasePos;     //释放魔炮的右边界位置
    private Transform curCannonReleasePos;
    public float jumpBackNeedTime;
    public float jumpBackStartAngle;    //后跳的起始角度
    public float g;
    private Vector2 startVelocity;
    private float curJumpBackResumeTime;    //当前后跳消耗的时间
    private Vector3 jumpBackStartPos;       //起跳时的起始位置
    public float cannonStorageTime;     //蓄力时间
    private float curCannonStorageTime;
    public float cannonLastTime;    //持续时间
    private float curCannonLastTime;
    public Collider2D cannonCheckArea;  //生效范围的碰撞体
    public GameObject magicCannonEffect;

    public override void OnStart()
    {
        player = (Owner.GetVariable("player") as SharedGameObject).Value;
        selfRigidbody = GetComponent<Rigidbody2D>();

        Owner.SetVariable("state", state);
        Owner.SetVariable("isJumpBackOver", (SharedBool)false);
        //面向Player
        transform.rotation = transform.position.x < player.transform.position.x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        //确定释放位置
        //向着远离Player的方向
        curCannonReleasePos = transform.position.x < player.transform.position.x ? cannonLeftReleasePos.Value : cannonRightReleasePos.Value;
        //开始后跳
        selfRigidbody.bodyType = RigidbodyType2D.Kinematic;
        selfRigidbody.velocity = Vector2.zero;
        //计算后跳抛物线所需要的变量
        jumpBackStartPos = transform.position;
        startVelocity.x = Mathf.Abs(curCannonReleasePos.position.x - transform.position.x) / jumpBackNeedTime;
        startVelocity.y = -0.5f * g * jumpBackNeedTime;
        curJumpBackResumeTime = 0;
        StartCoroutine(JumpBack());
        curCannonStorageTime = 0;
        curCannonLastTime = 0;
    }

    public override TaskStatus OnUpdate()
    {
        if (!(Owner.GetVariable("isJumpBackOver") as SharedBool).Value)
        {
            return TaskStatus.Running;
        }
        if (curCannonStorageTime < cannonStorageTime)
        {
            curCannonStorageTime += Time.deltaTime;
            return TaskStatus.Running;
        }
        //释放
        magicCannonEffect.transform.GetChild(1).gameObject.SetActive(true);
        cannonCheckArea.enabled = true;
        cannonCheckArea.GetComponent<MagicCannonCheckArea>().damage = damage;
        if (curCannonLastTime >= cannonLastTime)
        {
            //释放结束
            magicCannonEffect.transform.GetChild(1).gameObject.SetActive(false);
            magicCannonEffect.SetActive(false);
            cannonCheckArea.enabled = false;
            return TaskStatus.Success;
        }
        curCannonLastTime += Time.deltaTime;
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        selfRigidbody.bodyType = RigidbodyType2D.Dynamic;
        //
        Owner.SetVariable("canIdle", (SharedBool)true);
        Owner.SetVariable("canMagicCannon", (SharedBool)false);
        StartCoroutine(Util.instance.SetSharedValueAfterTimeToBT(cd, Owner, "canMagicCannon", (SharedBool)true));
    }

    public IEnumerator JumpBack()
    {
        while (Vector3.Distance(transform.position, curCannonReleasePos.position) >= 0.1f)
        {
            float x = startVelocity.x * curJumpBackResumeTime;
            float y = startVelocity.y * curJumpBackResumeTime + 0.5f * g * curJumpBackResumeTime * curJumpBackResumeTime;
            transform.position = jumpBackStartPos + new Vector3((transform.position.x < curCannonReleasePos.position.x ? 1 : -1) * x, y, 0);
            curJumpBackResumeTime += Time.fixedDeltaTime;
            yield return null;
        }
        Owner.SetVariable("isJumpBackOver", (SharedBool)true);
        //selfRigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
}
