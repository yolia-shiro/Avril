using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicStorageState : PlayerMagicBasicState
{
    public override void OnEnter(MagicSystem magicSystem, PlayerController playerController)
    {
        //时间变缓
        Time.timeScale = magicSystem.selectedScaleTime;
        //
        //magicSystem.selectedStorageMissileIndex = 0;
    }

    public override void OnUpdate(MagicSystem magicSystem, PlayerController playerController)
    {
        bool magicButton = Input.GetButton("Magic");
        bool magicStorageButton = Input.GetButton("Select Storage Missile");

        if (!magicStorageButton || magicSystem.magicStorge.Count == 0)
        {
            Time.timeScale = 1.0f;
            //切换回默认状态
            if (magicSystem.curMagic == null)
            {
                magicSystem.TranslateToState(magicSystem.magicDefaultState);
            }
            else 
            {
                //合并
                magicSystem.StartCoroutine(magicSystem.MergeMagic());

                magicSystem.TranslateToState(magicSystem.magicMissileState);
            }
            
            return;
        }

        if (playerController.isMagic && !magicButton)
        {
            magicSystem.MagicLaunch();
        }

        if (magicSystem.selectedStorageMagicIndex >= magicSystem.magicStorge.Count)
        {
            magicSystem.selectedStorageMagicIndex = magicSystem.magicStorge.Count - 1;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            magicSystem.selectedStorageMagicIndex--;
            magicSystem.selectedStorageMagicIndex = magicSystem.selectedStorageMagicIndex < 0 ? 0 : magicSystem.selectedStorageMagicIndex;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            magicSystem.selectedStorageMagicIndex++;
            magicSystem.selectedStorageMagicIndex = magicSystem.selectedStorageMagicIndex >= magicSystem.magicStorge.Count ? 
                                                        magicSystem.magicStorge.Count - 1 : magicSystem.selectedStorageMagicIndex;
        }

        //飞弹跟随
        if (magicSystem.curMagic != null)
        {
            magicSystem.curMagic.FollowCreatingPos(magicSystem.magicPos);
        }
    }
}
