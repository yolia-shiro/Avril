using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAssistState : PlayerMagicBasicState
{
    public override void OnEnter(MagicSystem magicSystem, PlayerController playerController)
    {
        playerController.isMagic = true;
        if (magicSystem.curAssistMagicKind == 0)
        {
            //治愈魔法
            playerController.magicKind = 2;
        }
        else if(magicSystem.curAssistMagicKind == 1)
        {
            //防御魔法
            playerController.magicKind = 3;
        }
        
        //生成魔法
        if (magicSystem.curMagic == null)
        {
            AssistMagic assistMagic = Object.Instantiate(magicSystem.assistMagicPrefabs[magicSystem.curAssistMagicKind], magicSystem.magicPos.position, Quaternion.identity).GetComponent<AssistMagic>();
            magicSystem.curMagic = assistMagic;
        }
    }

    public override void OnUpdate(MagicSystem magicSystem, PlayerController playerController)
    {
        //bool magicButton = Input.GetButton("Magic");
        bool magicButton = Input.GetKey(KeyCode.V);
        bool magicStorageButton = Input.GetButton("Select Storage Missile");

        if (playerController.isMagic && !magicButton)
        {
            //magicSystem.MagicLaunch();
            ////屏幕震动
            //magicSystem.StartCoroutine(MainCamera.instance.Shake(magicSystem.magicLaunchCameraDuration, magicSystem.magicLaunchCameraMigration));
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
    }
}
