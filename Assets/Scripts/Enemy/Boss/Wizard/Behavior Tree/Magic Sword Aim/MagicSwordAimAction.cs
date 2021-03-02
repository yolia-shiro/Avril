using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class MagicSwordAimAction : Action
{
    private Rigidbody2D selfRigidbody;

    public float cd;
    public SharedInt state;
    public float damage;
    public SharedTransform releasePos;
    private GameObject player;

    public GameObject swordPrefabs;
    public int swordGenerateNum;
    public float swordCreateRadius;
    private List<GameObject> swordLists = new List<GameObject>();

    public float aimSpeed;  //瞄准速度
    public float aimTime;   //瞄准时间，之后进行发射
    private float curAimTime;
    public float launchSpeed;   //发射速度

    public override void OnStart()
    {
        player = (Owner.GetVariable("player") as SharedGameObject).Value;
        selfRigidbody = GetComponent<Rigidbody2D>();

        Owner.SetVariable("state", state);
        //移动到指定位置
        Util.instance.MoveToTargetPos(transform, releasePos.Value.position, true);
        ////关闭重力
        selfRigidbody.bodyType = RigidbodyType2D.Kinematic;
        selfRigidbody.velocity = Vector2.zero;
        //生成魔力剑
        GenerateMagicSword();
        curAimTime = 0;
    }

    public override TaskStatus OnUpdate()
    {
        if (swordLists.Count != 0)
        {
            if (curAimTime < aimTime)
            {
                //打开第一把剑
                swordLists[0].SetActive(true);
                //瞄准Player
                Vector3 dir = (swordLists[0].transform.position - player.transform.position).normalized;
                swordLists[0].transform.up = Vector3.MoveTowards(swordLists[0].transform.up, dir, aimSpeed * Time.deltaTime);
                curAimTime += Time.deltaTime;
            }
            else
            {
                //发射
                swordLists[0].GetComponent<MagicSword>().SetAttrToStartLaunch(launchSpeed);
                swordLists.RemoveAt(0);
                curAimTime = 0;
            }
        }
        else
        {
            //打开重力
            selfRigidbody.bodyType = RigidbodyType2D.Dynamic;
            if ((Owner.GetVariable("isGround") as SharedBool).Value)
            {
                //结束当前状态
                selfRigidbody.bodyType = RigidbodyType2D.Dynamic;
                return TaskStatus.Success;
            }
        }
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        Owner.SetVariable("canIdle", (SharedBool)true);
        Owner.SetVariable("canMagicSwordAim", (SharedBool)false);
        StartCoroutine(Util.instance.SetSharedValueAfterTimeToBT(cd, Owner, "canMagicSwordAim", (SharedBool)true));
        //Owner.SetVariable("state", (SharedInt)0);
    }

    //获取随机生成位置
    public Vector2 GetRandomPos()
    {
        float r = Random.Range(0, swordCreateRadius);
        float angle = Random.Range(0, Mathf.PI);
        float x = r * Mathf.Cos(angle);
        float y = r * Mathf.Sin(angle);
        return new Vector2(x, y);
    }

    //魔法剑随机生成
    public void GenerateMagicSword()
    {
        //生成魔法剑
        for (int i = 0; i < swordGenerateNum; i++)
        {
            Vector2 GeneratePos = GetRandomPos();
            swordLists.Add(Object.Instantiate(swordPrefabs, releasePos.Value));
            swordLists[swordLists.Count - 1].transform.localPosition = GeneratePos;
            swordLists[swordLists.Count - 1].SetActive(false);
            swordLists[swordLists.Count - 1].GetComponent<MagicSword>().damage = damage;
        }
    }
}
