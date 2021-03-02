using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Water Magic Skill", menuName = "Magic Skill/New Water Magic Skill")]
public class BasicWaterMagicSkill : BasicMagicSkill
{
    [Header("Exclusive")]
    public int shootNum;    //蓄力之后发射的数量
    public List<Vector3> offsets;   //蓄力之后出现位置的偏移量（出现在敌人的头顶）
    //public int LockObjectNum;   //锁定对象数

    private Transform target;

    public override void StorageShoot(Vector3 shootPos, Vector3 dir)
    {
        //base.StorageShoot(shootPos, dir);
        //寻找目标
        SortedSet<Transform> targets = new SortedSet<Transform>();
        Util.instance.GetTransformsInCameraAreaSorted("Enemy", new Compare.TransformCompare(), out targets);
        foreach (var t in targets)
        {
            target = t;
            break;
        }
        //生成魔法
        for (int i = 0; i < shootNum; i++)
        {
            //生成对象
            GameObject go = Instantiate(storageShootPrefabs, target.position + offsets[i], Quaternion.identity);
            go.transform.localScale = storageShootScale;
            go.GetComponent<BasicWaterMagic>().SetAimTarget(target);
        }
    }
}
namespace Compare
{
    public class TransformCompare : IComparer<Transform>
    {
        public int Compare(Transform x, Transform y)
        {
            Vector3 player = GameManager.Instance.playerData.transform.position;
            float disX = Vector3.Distance(player, x.position);
            float disY = Vector3.Distance(player, y.position);
            if (disX < disY)
            {
                return -1;
            }
            else if (disX == disY)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }

}
