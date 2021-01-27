using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//存放用户数据信息
public class PlayerData : MonoBehaviour
{
    [Header("Basic Data")]
    public float maxHP;
    public float curHP;
    public float maxMP;
    public float curMP;
    public float maxBP;
    public float curBP;

    [Header("Magic Data")]
    public float curMagicAttachValue;


    private void Start()
    {
        curHP = maxHP;
        curMP = maxMP;
        curBP = maxBP;
    }

}
