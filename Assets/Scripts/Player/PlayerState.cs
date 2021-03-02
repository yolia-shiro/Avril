using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [Header("State")]
    public bool isGround;

    public bool isJump;

    public bool isRoll;

    public bool isAttack;

    public bool isCounterattacked;

    public bool isMagicResonance;

    public bool isMagic;
    public bool isStorage;

    public bool isHit;
    public int hitKind;

    public bool isDead;

    public bool isDownPlatform;
}
