using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerBasicState
{
    public override void OnEnter(PlayerController player)
    {

    }

    public override void OnFixedUpdate(PlayerController player)
    { 
        player.xAxis = Input.GetAxis("Horizontal");
        float speed = Mathf.Abs(player.xAxis) < 0.3 ? player.walkSpeed : player.runSpeed;
        player.Movement(speed);
    }

    public override void OnUpdate(PlayerController player)
    {
        if (!player.haveStorage && Input.GetButtonDown("Storage"))
        {
            //进入蓄力状态
            player.TranslateToState(player.playerStorageShootState);
            return;
        }
        if (Input.GetButtonDown("Element Release")) 
        {
            //元素释放
            player.ElementRelease();
        }
        if (Input.GetAxis("Vertical") < 0 && Input.GetButtonDown("Jump") && player.PlayerState.isGround)
        {
            //下穿单向地面
            PlatformEffector2D platform = null;
            foreach (var collider in Physics2D.OverlapBoxAll(player.bottomcCheckPoint.position, player.bottomCheckSize, player.groundLayer))
            {
                platform = null;
                if ((platform = collider.GetComponent<PlatformEffector2D>()) != null)
                {
                    //取消单向平台为Player的支持
                    player.StartCoroutine(player.UpdatePlatform(platform));
                }
            }
        }
        else if (player.PlayerState.isGround && Input.GetButtonDown("Jump"))
        {
            //切换到跳跃状态
            player.TranslateToState(player.playerJumpState);
            return;
        }
        if (Input.GetButtonDown("Roll"))
        {
            player.TranslateToState(player.playerRollState);
            return;
        }
        if (Input.GetButtonDown("Attack"))
        {
            player.TranslateToState(player.playerAttackState);
            return;
        }
        if (player.PlayerData.CurResonanceValue < player.PlayerData.MaxResonanceValue && Input.GetButton("Resonance"))
        {
            player.TranslateToState(player.playerResonanceState);
            return;
        }
        if (Input.GetButtonDown("Magic"))
        {
            player.TranslateToState(player.playerShootState);
            return;
        }
    }
}
