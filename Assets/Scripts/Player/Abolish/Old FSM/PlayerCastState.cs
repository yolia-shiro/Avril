//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 施法状态
///// </summary>
//public class PlayerCastState : PlayerBasicState
//{
//    public override void OnEnter(PlayerController player)
//    {
//        //player.isMagic = true;
//        //player.magicKind = 0;
//    }

//    public override void OnFixedUpdate(PlayerController player)
//    {
//        player.xAxis = Input.GetAxis("Horizontal");
//        float speed = Mathf.Abs(player.xAxis) < 0.3 ? player.walkSpeed : player.runSpeed;
//        player.Movement(speed);
//    }

//    public override void OnUpdate(PlayerController player)
//    {
//        if (Input.GetButtonDown("Switch Cast State"))
//        {
//            //离开施法状态
//            player.isMagic = false;
//            player.TranslateToState(player.playerMoveState);
//            return;
//        }
//        if (!player.haveStorage && Input.GetButtonDown("Storage"))
//        {
//            //进入蓄力状态
//            player.TranslateToState(player.playerStorageShootState);
//            return;
//        }
//        if (Input.GetButtonDown("Element Release"))
//        {
//            //元素释放
//            player.ElementRelease();
//        }
//        if (player.isGround && Input.GetButtonDown("Jump"))
//        {
//            //切换到跳跃状态
//            player.TranslateToState(player.playerJumpState);
//            return;
//        }
//        if (Input.GetButtonDown("Roll"))
//        {
//            player.TranslateToState(player.playerRollState);
//            return;
//        }
//        if (Input.GetButtonDown("Attack"))
//        {
//            player.TranslateToState(player.playerAttackState);
//            return;
//        }
//        if (player.curResonanceValue < player.maxResonanceValue && Input.GetButton("Resonance"))
//        {
//            player.TranslateToState(player.playerResonanceState);
//            return;
//        }
//        if (Input.GetButtonDown("Magic"))
//        {
//            player.TranslateToState(player.playerShootState);
//            return;
//        }
//    }
//}
