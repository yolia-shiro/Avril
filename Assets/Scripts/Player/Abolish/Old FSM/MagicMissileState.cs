//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MagicMissileState : PlayerMagicBasicState
//{
//    public override void OnEnter(MagicSystem magicSystem, PlayerController playerController)
//    {
//        //产生攻击魔法
//        magicSystem.GenerateAttackMagic();
//    }

//    public override void OnUpdate(MagicSystem magicSystem, PlayerController playerController)
//    {
//        bool magicButton = Input.GetButton("Magic");
//        bool magicStorageButton = Input.GetButton("Select Storage Missile");
        
//        if (playerController.isMagic && !magicButton)
//        {
//            //释放攻击魔法(发射)
//            magicSystem.MagicLaunch();
//            //屏幕震动
//            magicSystem.StartCoroutine(MainCamera.instance.Shake(magicSystem.magicLaunchCameraDuration, magicSystem.magicLaunchCameraMigration));
//            //切换状态
//            if (!magicStorageButton)
//            {
//                magicSystem.TranslateToState(magicSystem.magicDefaultState);
//            }
//            else 
//            {
//                magicSystem.TranslateToState(magicSystem.magicStorageState);
//            }
            
//            return;
//        }


//        if (!magicStorageButton || magicSystem.magicStorge.Count == 0)
//        {
//            //魔法融合
//            Time.timeScale = 1.0f;
//            if (Input.GetButtonUp("Select Storage Missile"))
//            {
//                magicSystem.StartCoroutine(magicSystem.MergeMagic());
//            }
//        }

//        //武器附魔
//        if (magicSystem.curMagic != null && Input.GetKeyUp(KeyCode.LeftShift))
//        {
//            Debug.Log("武器附魔");
//            //武器附魔
//            magicSystem.WeaponMagicAttach();
//            //切换状态
//            magicSystem.TranslateToState(magicSystem.magicDefaultState);
//            return;
//        }

//        //存储魔法
//        if (magicSystem.curMagic != null && Input.GetButtonDown("Magic Storage") && magicSystem.magicStorge.Count < magicSystem.storagePos.Count)
//        {
//            magicSystem.curMagic.SwitchMissileState(Magic.MagicState.ToStorage);
//            Magic tempMagic = magicSystem.curMagic;
//            magicSystem.curMagic = null;

//            magicSystem.StartCoroutine(magicSystem.MissileToStoragePos(tempMagic));

//            //playerController.isMagic = false;
//            //playerController.magicKind = -1;

//            //切换回默认状态
//            magicSystem.TranslateToState(magicSystem.magicDefaultState);
//            return;
//        }

        
//        if (magicSystem.curMagic != null)
//        {
//            //飞弹跟随
//            magicSystem.curMagic.FollowCreatingPos(magicSystem.magicPos);
//            //消耗魔力
//            magicSystem.ResumeMagic(magicSystem.curMagic.magicConsumptionPerFrame);
//        }
//    }
//}
