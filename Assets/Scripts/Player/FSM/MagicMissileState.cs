using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissileState : PlayerMagicBasicState
{
    public override void OnEnter(MagicSystem magicSystem, PlayerController playerController)
    {
        playerController.isMagic = true;
        playerController.magicKind = 0;
        //产生飞弹
        if (magicSystem.missile == null)
        {
            magicSystem.missile = Object.Instantiate(magicSystem.missilePrefabs, magicSystem.missilePos.position, Quaternion.identity).GetComponent<Missile>();
        }
    }

    public override void OnUpdate(MagicSystem magicSystem, PlayerController playerController)
    {
        bool magicButton = Input.GetButton("Magic");
        bool magicStorageButton = Input.GetButton("Select Storage Missile");

        if (playerController.isMagic && !magicButton)
        {
            magicSystem.MagicLaunch();
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

        if (!magicStorageButton || magicSystem.missileStorge.Count == 0)
        {
            Time.timeScale = 1.0f;
        }

        if (magicSystem.missile != null && Input.GetButtonDown("Magic Storage") && magicSystem.missileStorge.Count < magicSystem.storagePos.Count)
        {
            magicSystem.missile.SwitchMissileState(Missile.missileState.Storage);
            //放置到Magic Storage处
            magicSystem.missile.transform.parent = magicSystem.storagePos[magicSystem.missileStorge.Count];
            magicSystem.missile.transform.localPosition = Vector3.zero;
            magicSystem.missile.transform.localScale = Vector3.one;
            magicSystem.missileStorge.Add(magicSystem.missile.gameObject);
            magicSystem.StartCoroutine(magicSystem.DestoryStorageMissileAfterTime());

            magicSystem.missile = null;

            playerController.isMagic = false;
            playerController.magicKind = -1;

            //切换回默认状态
            magicSystem.TranslateToState(magicSystem.magicDefaultState);
            return;
        }

        //飞弹跟随
        if (magicSystem.missile != null)
        {
            magicSystem.missile.FollowCreatingPos(magicSystem.missilePos);
        }
    }
}
