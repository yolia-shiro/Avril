﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissileState : PlayerMagicBasicState
{
    public override void OnEnter(MagicSystem magicSystem, PlayerController playerController)
    {
        playerController.isMagic = true;
        playerController.magicKind = 0;
        //产生飞弹
        if (magicSystem.curMagic == null)
        {
            AttackMagic attackMagic = Object.Instantiate(magicSystem.attackMagicPrefabs[magicSystem.curAttackMagicKind], magicSystem.magicPos.position, Quaternion.identity).GetComponent<AttackMagic>();
            attackMagic.SetAttackMagicType(magicSystem.curAttackMagicKind);
            magicSystem.curMagic = attackMagic;
        }
    }

    public override void OnUpdate(MagicSystem magicSystem, PlayerController playerController)
    {
        bool magicButton = Input.GetButton("Magic");
        bool magicStorageButton = Input.GetButton("Select Storage Missile");
        
        if (playerController.isMagic && !magicButton)
        {
            magicSystem.MagicLaunch();
            //屏幕震动
            magicSystem.StartCoroutine(MainCamera.instance.Shake(magicSystem.magicLaunchCameraDuration, magicSystem.magicLaunchCameraMigration));
            //切换状态
            if (!magicStorageButton)
            {
                magicSystem.TranslateToState(magicSystem.magicDefaultState);
            }
            else 
            {
                magicSystem.TranslateToState(magicSystem.magicStorageState);
            }
            
            return;
        }


        if (!magicStorageButton || magicSystem.magicStorge.Count == 0)
        {
            Time.timeScale = 1.0f;
            if (Input.GetButtonUp("Select Storage Missile"))
            {
                magicSystem.StartCoroutine(magicSystem.MergeMagic());
            }
        }

        //存储魔法
        if (magicSystem.curMagic != null && Input.GetButtonDown("Magic Storage") && magicSystem.magicStorge.Count < magicSystem.storagePos.Count)
        {
            magicSystem.curMagic.SwitchMissileState(Magic.MagicState.ToStorage);
            Magic tempMagic = magicSystem.curMagic;
            magicSystem.curMagic = null;

            magicSystem.StartCoroutine(magicSystem.MissileToStoragePos(tempMagic));

            playerController.isMagic = false;
            playerController.magicKind = -1;

            //切换回默认状态
            magicSystem.TranslateToState(magicSystem.magicDefaultState);
            return;
        }

        //飞弹跟随
        if (magicSystem.curMagic != null)
        {
            magicSystem.curMagic.FollowCreatingPos(magicSystem.magicPos);
        }
    }
}
