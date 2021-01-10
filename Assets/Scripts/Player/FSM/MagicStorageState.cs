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

        if (!magicStorageButton || magicSystem.missileStorge.Count == 0)
        {
            Time.timeScale = 1.0f;
            //切换回默认状态
            if (magicSystem.missile == null)
            {
                magicSystem.TranslateToState(magicSystem.magicDefaultState);
            }
            else 
            {
                magicSystem.TranslateToState(magicSystem.magicMissileState);
            }
            
            return;
        }

        if (playerController.isMagic && !magicButton)
        {
            magicSystem.MagicLaunch();
        }

        if (magicSystem.selectedStorageMissileIndex >= magicSystem.missileStorge.Count)
        {
            magicSystem.selectedStorageMissileIndex = magicSystem.missileStorge.Count - 1;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            magicSystem.selectedStorageMissileIndex--;
            magicSystem.selectedStorageMissileIndex = magicSystem.selectedStorageMissileIndex < 0 ? 0 : magicSystem.selectedStorageMissileIndex;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            magicSystem.selectedStorageMissileIndex++;
            magicSystem.selectedStorageMissileIndex = magicSystem.selectedStorageMissileIndex >= magicSystem.missileStorge.Count ? 
                                                        magicSystem.missileStorge.Count - 1 : magicSystem.selectedStorageMissileIndex;
        }
        //
        //Debug.Log(magicSystem.selectedStorageMissileIndex + " : " + magicSystem.missileStorge[magicSystem.selectedStorageMissileIndex].transform.parent.name);
        //飞弹跟随
        if (magicSystem.missile != null)
        {
            magicSystem.missile.FollowCreatingPos(magicSystem.missilePos);
        }
    }
}
