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
            AssistMagic assistMagic = null;
            switch (magicSystem.curAssistMagicKind) 
            {
                case 0:
                    assistMagic = Object.Instantiate(magicSystem.assistMagicPrefabs[magicSystem.curAssistMagicKind], 
                        magicSystem.healPos.position, Quaternion.identity).GetComponent<AssistMagic>();
                    break;
                case 1:
                    assistMagic = Object.Instantiate(magicSystem.assistMagicPrefabs[magicSystem.curAssistMagicKind],
                        magicSystem.defensePos.position, Quaternion.identity).GetComponent<AssistMagic>();
                    break;
            }
            magicSystem.curMagic = assistMagic;
        }
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
            return;
        }
        

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
