using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Magic Skill", menuName = "Magic Skill/New Magic Skill")]
/// <summary>
/// 基础魔法
/// 其他魔法都是在此基础上继承实现的
/// </summary>
public class BasicMagicSkill : ScriptableObject
{
    [Header("Shoot")]
    public bool canShoot;   //是否能够进行普通释放
    public float resumeMPInNormal;  //正常射击下的耗魔量
    //public float resumeResonanceValueInNoraml;  //正常射击下的消耗共鸣值
    public GameObject shootPrefabs;

    [Header("Storage Shoot")]
    public bool canStorageShoot;    //是否能够进行蓄力释放
    public float resumeMPInStorage; //蓄力下的耗魔量
    public GameObject storagePrefabs;   //储存状态下的预制体
    public GameObject storageShootPrefabs;
    public Vector3 storageScale;  //储存状态下的大小
    public Vector3 storageShootScale;   //储存魔法释放时的大小
    public float reactionVelocity;
    public AnimationCurve reactionVelocityCurve;     //反作用力的速度曲线
    //protected Transform player;
    //private void OnEnable()
    //{
    //    player = FindObjectOfType<PlayerController>().transform;
    //}
    
    //TODO:将函数从ScriptableObject中移除
    public virtual void NormalShoot(Vector3 shootPos, Vector3 dir) 
    {
        GameObject go = Object.Instantiate(shootPrefabs, shootPos, Quaternion.identity);
        //go.transform.rotation = dir == 1 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        go.transform.right = dir;
    }
    //TODO:将函数从ScriptableObject中移除
    public virtual void StorageShoot(Vector3 shootPos, Vector3 dir) 
    {
        GameObject go = Object.Instantiate(storageShootPrefabs, shootPos, Quaternion.identity);
        go.transform.localScale = storageShootScale;
        go.transform.right = dir;
    }
}
