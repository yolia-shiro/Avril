using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Charactor Data/Player Data")]
public class PlayerStatus_SO : ScriptableObject
{
    [Header("Basic Data")]
    public float maxHP;
    public float curHP;
    public float maxMP;
    public float curMP;
    public float restoreMPPerFrame;
    public float maxBP;
    public float curBP;
    public int curResonanceType;
    public float maxResonanceValue;     //总共鸣值
    public float curResonanceValue;    //当前共鸣值
    public float resonanceRadius;   //共鸣半径
    
}
