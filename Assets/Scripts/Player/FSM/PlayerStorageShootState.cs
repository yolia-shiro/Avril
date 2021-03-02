using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 蓄力魔法状态
/// </summary>
public class PlayerStorageShootState : PlayerBasicState
{
    private GameObject go;
    private Vector3 targetScale;
    private BasicMagicSkill basicMagicSkill;
    private float scaleSpeed;

    public override void OnEnter(PlayerController player)
    {
        if (player.storagePos.childCount != 0)
        {
            return;
        }

        player.PlayerState.isStorage = true;
        player.curResumeMagic = 0;
        basicMagicSkill = player.magicSkillLists[player.curMagicSkillIndex];
        targetScale = basicMagicSkill.storageScale;
        scaleSpeed = targetScale.magnitude * player.resumeMagicPerFrame / basicMagicSkill.resumeMPInStorage;
        
        go = Object.Instantiate(basicMagicSkill.storagePrefabs, player.storagePos);
        go.transform.localScale = Vector3.zero;
    }

    public override void OnFixedUpdate(PlayerController player)
    {
        player.xAxis = Input.GetAxis("Horizontal");
        if (player.xAxis <= -0.3f)
        {
            player.xAxis = -0.29f;
        }
        else if (player.xAxis >= 0.3f) 
        {
            player.xAxis = 0.29f;
        }
        float speed = player.walkSpeed;
        player.Movement(speed);
    }

    public override void OnUpdate(PlayerController player)
    {
        if (Input.GetButtonDown("Jump"))
        {
            //切换到跳跃状态
            player.TranslateToState(player.playerJumpState);
            return;
        }
        if (Input.GetButtonDown("Roll"))
        {
            player.TranslateToState(player.playerRollState);
            return;
        }
        if (Input.GetButtonDown("Attack"))
        {
            player.TranslateToState(player.playerAttackState);
            return;
        }
        if (player.IsStorageMagicOver())
        {
            //蓄力结束
            player.PlayerState.isStorage = false;
            player.TranslateToState(player.playerMoveState);
            return;
        }
        if (Input.GetButtonDown("Storage"))
        {
            //中途结束蓄力状态
            player.PlayerState.isStorage = false;
            //返还消耗的魔力和共鸣值
            player.MagicResume(player.curResumeMagic);
            //消除蓄力的标识
            player.ClearStoragePos();
            //切换状态
            player.TranslateToState(player.playerMoveState);
            return;
        }
        if (!player.canMagic(-player.resumeMagicPerFrame * Time.deltaTime))
        {
            //魔力不足，保持当前状态，等待满足下一次蓄力的魔力和共鸣值
            return;
        }
        //消耗魔力和共鸣值
        player.MagicResume(-player.resumeMagicPerFrame * Time.deltaTime);
        //记录消耗信息
        player.curResumeMagic += player.resumeMagicPerFrame * Time.deltaTime;
        go.transform.localScale = Vector3.MoveTowards(go.transform.localScale, targetScale, scaleSpeed * Time.deltaTime);
    }
}
