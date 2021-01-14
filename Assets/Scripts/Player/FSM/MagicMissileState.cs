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
        if (magicSystem.curMagic == null)
        {
            AttackMagic attackMagic = Object.Instantiate(magicSystem.magicPrefabs[magicSystem.curMagicKind], magicSystem.magicPos.position, Quaternion.identity).GetComponent<AttackMagic>();
            attackMagic.SetAttackMagicType(magicSystem.curMagicKind);
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

            magicSystem.StartCoroutine(MissileToStoragePos(magicSystem, tempMagic));

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

    public IEnumerator MissileToStoragePos(MagicSystem magicSystem, Magic toStorageMagic)
    {
        //提前设置父物体，以防连续存储时，发生储存位置重叠的问题
        magicSystem.storageIndex++;
        toStorageMagic.transform.parent = magicSystem.storagePos[magicSystem.storageIndex];
        //消散
        yield return magicSystem.StartCoroutine(toStorageMagic.MissileToTargetScale(Vector3.zero));
        //放置到Magic Storage处
        toStorageMagic.SwitchMissileState(Magic.MagicState.Storage);
        toStorageMagic.transform.localPosition = Vector3.zero;
        toStorageMagic.storageBeginLocalPos = toStorageMagic.transform.localPosition;
        toStorageMagic.randomDir = toStorageMagic.GetRandomPosInSphere();
        //还原到储存大小
        yield return toStorageMagic.StartCoroutine(toStorageMagic.MissileToTargetScale(Vector3.one));
        //进行数据存储
        //防止出现未存储完毕的魔法被用去混合
        magicSystem.magicStorge.Add(toStorageMagic.gameObject);

        magicSystem.StartCoroutine(magicSystem.DestoryStorageMissileAfterTime());
    }
}
