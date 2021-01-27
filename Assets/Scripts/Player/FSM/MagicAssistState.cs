using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAssistState : PlayerMagicBasicState
{
    public override void OnEnter(MagicSystem magicSystem, PlayerController playerController)
    {
        //生成辅助魔法
        magicSystem.GenerateAssistMagic();
    }

    public override void OnUpdate(MagicSystem magicSystem, PlayerController playerController)
    {
        bool magicButton = Input.GetKey(KeyCode.V);
        #region 储存相关(暂时废弃)
        //bool magicStorageButton = Input.GetButton("Select Storage Missile");
        #endregion

        AssistMagic assistMagic = null;

        if ((playerController.isMagic && !magicButton)
            || (magicSystem.curMagic != null && magicSystem.curMagic is AssistMagic && (assistMagic = ((AssistMagic)magicSystem.curMagic)).isOver))
        {
            #region 储存相关(暂时废弃)
            //切换状态
            //if (!magicStorageButton)
            //{
            //    magicSystem.TranslateToState(magicSystem.magicDefaultState);
            //}
            //else
            //{
            //    magicSystem.TranslateToState(magicSystem.magicStorageState);
            //}
            #endregion
            magicSystem.TranslateToState(magicSystem.magicDefaultState);
            if (magicSystem.curMagic != null)
            {
                magicSystem.curMagic.SwitchMissileState(Magic.MagicState.Effective);
            }
            magicSystem.curMagic = null;
            return;
        }

        //消耗魔力
        magicSystem.ResumeMagic(magicSystem.curMagic.magicConsumptionPerFrame);

        #region 储存相关(暂时废弃)
        //if (!magicStorageButton || magicSystem.magicStorge.Count == 0)
        //{
        //    Time.timeScale = 1.0f;
        //    if (Input.GetButtonUp("Select Storage Missile"))
        //    {
        //        magicSystem.StartCoroutine(magicSystem.MergeMagic());
        //    }
        //}

        ////存储魔法
        //if (magicSystem.curMagic != null && Input.GetButtonDown("Magic Storage") && magicSystem.magicStorge.Count < magicSystem.storagePos.Count)
        //{
        //    magicSystem.curMagic.SwitchMissileState(Magic.MagicState.ToStorage);
        //    Magic tempMagic = magicSystem.curMagic;
        //    magicSystem.curMagic = null;

        //    magicSystem.StartCoroutine(magicSystem.MissileToStoragePos(tempMagic));

        //    playerController.isMagic = false;
        //    playerController.magicKind = -1;

        //    //切换回默认状态
        //    magicSystem.TranslateToState(magicSystem.magicDefaultState);
        //    return;
        //}
        #endregion
    }
}
