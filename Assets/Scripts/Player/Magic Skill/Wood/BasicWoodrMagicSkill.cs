using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wood Magic Skill", menuName = "Magic Skill/New Wood Magic Skill")]
public class BasicWoodrMagicSkill : BasicMagicSkill
{
    [Header("Exclusive")]
    public int storageNum;  //蓄力魔法释放的个数
    public List<float> offsetAngle = new List<float>();     //初始释放时候的偏移角度

    public override void StorageShoot(Vector3 shootPos, Vector3 dir)
    {
        for (int i = 0; i < storageNum; i++) 
        {
            GameObject go = Instantiate(storageShootPrefabs, shootPos, Quaternion.identity);
            go.transform.localScale = storageShootScale;
            go.transform.right = new Vector3(Mathf.Cos((offsetAngle[i] + (dir.x < 0 ? 180 : 0)) * Mathf.Deg2Rad), 
                        Mathf.Sin((offsetAngle[i] + (dir.x < 0 ? 180 : 0)) * Mathf.Deg2Rad), 
                        0).normalized;
        }
    }
}
